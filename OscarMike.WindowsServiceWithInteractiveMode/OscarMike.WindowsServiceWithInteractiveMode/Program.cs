using OscarMike.Common.WindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OscarMike.WindowsServiceWithInteractiveMode
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WindowsServiceStarter.Run<SampleWindowsService>(SampleWindowsService.Name, SampleWindowsService.Description);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
