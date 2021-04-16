using System.Threading.Tasks;

namespace StartPos.Interfaces
{
    public interface ICronService
    {
        Task InstallCron();

        Task ConfigureCron();

        Task DisableCron();
    }
}