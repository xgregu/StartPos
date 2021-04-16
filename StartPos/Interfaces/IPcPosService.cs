using System.Threading.Tasks;

namespace StartPos.Interfaces
{
    public interface IPcPosService
    {
        Task StartPcPos();

        Task StartDiagnostic();

        void UpdateInsoftUpdateFiles();

        void ClientMonitorUpdateFiles();

        void KillPcPos();
    }
}