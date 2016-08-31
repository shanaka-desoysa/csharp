using System;

namespace OscarMike.Common.WindowsService
{
    public interface IWindowsService : IDisposable
    {
        void Start();
    }
}
