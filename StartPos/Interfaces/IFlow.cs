using StartPos.Enums;
using System.Threading.Tasks;

namespace StartPos.Interfaces
{
    public interface IFlow
    {
        string Name { get; }

        Task Run();

        UpdaterMode UpdateMode { get; }
    }
}