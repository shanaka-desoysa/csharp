using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace OscarMike.Common.WindowsService
{
    static class WindowsServiceInstaller
    {
        public static void Install(string serviceName, string serviceDescription)
        {
            CreateInstaller(serviceName, serviceDescription).Install(new Hashtable());
        }

        public static void Uninstall(string serviceName, string serviceDescription)
        {
            CreateInstaller(serviceName, serviceDescription).Uninstall(null);
        }

        private static Installer CreateInstaller(string serviceName, string serviceDescription)
        {
            var installer = new TransactedInstaller();
            installer.Installers.Add(new ServiceInstaller
            {
                ServiceName = serviceName,
                DisplayName = serviceName,
                Description = serviceDescription,
                StartType = ServiceStartMode.Automatic
            });
            installer.Installers.Add(new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem
            });
            var installContext = new InstallContext(serviceName + ".install.log", null);
            installContext.Parameters["assemblypath"] = Assembly.GetEntryAssembly().Location;
            installer.Context = installContext;
            return installer;
        }
    }
}
