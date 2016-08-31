using CERTENROLLLib;
using System;
using System.Security.Cryptography.X509Certificates;

namespace OscarMike.Common.Cryptography
{
    static public class SelfSignedCertificate
    {
        static public X509Certificate2 GenerateRootCertificate(string subjectName, TimeSpan expirationLength, string saveFile = null, string savePasword = null)
        {
            return GenerateCertificate(subjectName, expirationLength, null,  saveFile, savePasword);
        }

        static public X509Certificate2 GenerateCertificateSignedByRoot(string subjectName, TimeSpan expirationLength, X509Certificate2 rootCert, string saveFile = null, string savePasword = null)
        {
            if (rootCert == null)
            {
                throw new ArgumentNullException("rootCert is null.");
            }
            return GenerateCertificate(subjectName, expirationLength, rootCert, saveFile, savePasword);
        }

        static private X509Certificate2 GenerateCertificate(string subjectName, TimeSpan expirationLength, X509Certificate2 rootCert, string saveFile = null, string savePasword = null)
        {
            // create DN for subject and issuer
            var dn = new CX500DistinguishedName();
            dn.Encode("CN=" + subjectName, X500NameFlags.XCN_CERT_NAME_STR_NONE);

            var privateKey = new CX509PrivateKey();
            privateKey.ProviderName = "Microsoft Strong Cryptographic Provider";
            //privateKey.ProviderName = "Microsoft Base Cryptographic Provider v1.0";
            privateKey.Length = 2048;
            privateKey.KeySpec = X509KeySpec.XCN_AT_KEYEXCHANGE;
            privateKey.KeyUsage = X509PrivateKeyUsageFlags.XCN_NCRYPT_ALLOW_DECRYPT_FLAG | X509PrivateKeyUsageFlags.XCN_NCRYPT_ALLOW_KEY_AGREEMENT_FLAG;
            privateKey.MachineContext = true;
            privateKey.ExportPolicy = X509PrivateKeyExportFlags.XCN_NCRYPT_ALLOW_PLAINTEXT_EXPORT_FLAG;
            privateKey.Create();

            // Use the stronger SHA512 hashing algorithm
            var hashobj = new CObjectId();
            hashobj.InitializeFromAlgorithmName(ObjectIdGroupId.XCN_CRYPT_HASH_ALG_OID_GROUP_ID,
                ObjectIdPublicKeyFlags.XCN_CRYPT_OID_INFO_PUBKEY_ANY,
                AlgorithmFlags.AlgorithmFlagsNone, "SHA512");

            // Create the self signing request
            var cert = new CX509CertificateRequestCertificate();
            cert.InitializeFromPrivateKey(X509CertificateEnrollmentContext.ContextMachine, privateKey, "");
            cert.Subject = dn;
            cert.Issuer = dn; // the issuer and the subject are the same
            cert.NotBefore = DateTime.Now.Date;

            // This cert expires immediately. Change to whatever makes sense for you
            cert.NotAfter = cert.NotBefore + expirationLength;

            // Specify the hashing algorithm
            cert.HashAlgorithm = hashobj;

            // Encode the certificate
            cert.Encode();

            // Sign with root certificate
            if (rootCert != null)
            {
                ISignerCertificate signerCertificate = new CSignerCertificate();
                signerCertificate.Initialize(true, X509PrivateKeyVerify.VerifyNone, EncodingType.XCN_CRYPT_STRING_HEX, rootCert.GetRawCertDataString());

                cert.SignerCertificate = (CSignerCertificate)signerCertificate;
            }

            // Do the final enrollment process
            var enroll = new CX509Enrollment();
            // Load the certificate
            enroll.InitializeFromRequest(cert);
            // Optional: add a friendly name
            enroll.CertificateFriendlyName = subjectName;
            // Output the request in base64
            string csr = enroll.CreateRequest();
            // And install it back as the response
            // no password
            enroll.InstallResponse(InstallResponseRestrictionFlags.AllowUntrustedCertificate, csr, EncodingType.XCN_CRYPT_STRING_BASE64, "");
            // Output a base64 encoded PKCS#12 so we can import it back to the .Net security classes
            // No password, this is for internal consumption
            var base64encoded = enroll.CreatePFX("", PFXExportOptions.PFXExportChainWithRoot);

            // Instantiate the target class with the PKCS#12 data (and the empty password)
            // Mark the private key as exportable (this is usually what you want to do)
            // ark private key to go into the Machine store instead of the current users store

            var returnCert = new System.Security.Cryptography.X509Certificates.X509Certificate2(System.Convert.FromBase64String(base64encoded), "",
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            // Save the file
            if (saveFile != null)
            {
                byte[] certData;
                if (savePasword != null)
                {
                    certData = returnCert.Export(X509ContentType.Pfx, savePasword);
                }
                else
                {
                    certData = returnCert.Export(X509ContentType.Pfx);
                }

                System.IO.File.WriteAllBytes(saveFile, certData);
            }

            return returnCert;
        }
    }
}
