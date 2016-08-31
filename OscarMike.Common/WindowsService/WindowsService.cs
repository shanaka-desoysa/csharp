using System;
using System.ServiceProcess;

namespace OscarMike.Common.WindowsService
{
    public class WindowsService<T> : ServiceBase where T : IWindowsService, new()
    {
        private IWindowsService _service;

        protected override void OnStart(string[] args)
        {
            try
            {
                this.AutoLog = false;
                _service = new T();
                _service.Start();
            }
            catch (Exception)
            {
                ExitCode = 1064;
                throw;
            }
        }

        protected override void OnStop()
        {
            _service.Dispose();
        }
    }
}
