using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;

namespace OscarMike.Common.WindowsService
{
    public static class WindowsServiceStarter
    {
        public static void Run<T>(string serviceName, string serviceDescription) where T : IWindowsService, new()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (EventLog.SourceExists(serviceName))
                {
                    EventLog.WriteEntry(serviceName,
                        "Fatal Exception : " + Environment.NewLine + e.ExceptionObject, EventLogEntryType.Error);
                }
            };

            if (Environment.UserInteractive)
            {
                var cmd = (Environment.GetCommandLineArgs().Skip(1).FirstOrDefault() ?? "").ToLower();
                switch (cmd)
                {
                    case "-i":
                    case "-install":
                        Console.WriteLine("Installing {0}", serviceName);
                        WindowsServiceInstaller.Install(serviceName, serviceDescription);
                        break;
                    case "-u":
                    case "-uninstall":
                        Console.WriteLine("Uninstalling {0}", serviceName);
                        WindowsServiceInstaller.Uninstall(serviceName, serviceDescription);
                        break;
                    default:
#if DEBUG
                        using (var service = new T())
                        {
                            service.Start();
                            Console.WriteLine("Running {0}, press any key to stop", serviceName);
                            Console.ReadKey();
                        }
#endif
                        break;
                }
            }
            else
            {
                ServiceBase.Run(new WindowsService<T> { ServiceName = serviceName });
            }
        }
    }
}
