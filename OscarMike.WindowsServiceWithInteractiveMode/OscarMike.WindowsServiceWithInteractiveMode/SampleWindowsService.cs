using OscarMike.Common.WindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarMike.WindowsServiceWithInteractiveMode
{
    class SampleWindowsService : IWindowsService
    {
        public const string Name = "Sample Windows Service";
        public const string Description = "Sample Windows Service. Hello World!";

        public SampleWindowsService()
        {
        }
        void IDisposable.Dispose()
        {
            Console.WriteLine("Hello World! Stopping...");
        }

        void IWindowsService.Start()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
