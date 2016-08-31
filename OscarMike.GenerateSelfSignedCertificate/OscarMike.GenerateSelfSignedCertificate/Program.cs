using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OscarMike.Common.Cryptography;

namespace OscarMike.GenerateSelfSignedCertificate
{
    class Program
    {
        static void Main(string[] args)
        {
            // Generate root certificate
            var rootCert = SelfSignedCertificate.GenerateRootCertificate("TestRootCertificate", TimeSpan.FromDays(30));

            // Generate certificate signed by root
            var signedCert = SelfSignedCertificate.GenerateCertificateSignedByRoot("TestSignedCertificate", TimeSpan.FromDays(30), rootCert);

            // Save certificates
            //var certData = rootCert.Export(X509ContentType.Pfx, "Datacard1$");
            //File.WriteAllBytes(@"C:\Test Certs\CMRoot.pfx", certData);

            // Generate root certificate and save with password
            var rootCert2 = SelfSignedCertificate.GenerateRootCertificate("TestRootCertificate", TimeSpan.FromDays(30), @"C:\tmp\testroot.pfx", "test1234");

            // Generate certificate signed by root and save with password
            var signedCert2 = SelfSignedCertificate.GenerateCertificateSignedByRoot("TestSignedCertificate", TimeSpan.FromDays(30), rootCert, @"C:\tmp\testsigned.pfx", "test1234");
        }
    }
}
