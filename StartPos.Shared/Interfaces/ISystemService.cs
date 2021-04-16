using System;
using System.Threading.Tasks;

namespace StartPos.Shared.Interfaces
{
    public interface ISystemService
    {
        void Restart();

        void Shutdown();

        Task DisableSystemSleep();

        DateTime SystemUpTime();

        long GetDirectorySizeMb(string path);

        long GetFreeDiskSpaceMb(string path);

        void ClearTempDirectory();
    }
}