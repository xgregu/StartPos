using Csc.ApiClient.Models;

namespace StartPos.Interfaces
{
    public interface IContext
    {
        string AppVersion { get; }
        string TempPath { get; }
        IPcPosInfo PcPosInfo { get; }
        string ActiveFlow { get; set; }
        Report TsReport { get; set; }
    }
}