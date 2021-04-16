using System.Threading.Tasks;

namespace StartPos.Interfaces
{
    public interface IMenuStartService
    {
        Task InstallClassicShell();

        Task ConfigureClassicShell();
    }
}