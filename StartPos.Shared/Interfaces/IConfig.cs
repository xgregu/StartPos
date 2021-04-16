using StartPos.Shared.Configs;
using System.Collections.Generic;

namespace StartPos.Shared.Interfaces
{
    public interface IConfig
    {
        bool IsStartPosEnabled { get; set; }
        string AppToRun { get; set; }
        Contractor Contractor { get; set; }
        int PosNumber { get; set; }
        bool IsClientMonitorInstalled { get; set; }
        bool IsInsoftUpdateServerInstalled { get; set; }
        bool IsAllDayShop { get; set; }
        Database Database { get; set; }
        ObligatoryBackup ObligatoryBackup { get; set; }
        AlternativeBackup AlternativeBackup { get; set; }
        List<DataItem> DataBackupItems { get; set; }
        CscApiClientConfig ApiClientConfig { get; set; }
        UpdaterConfig Update { get; set; }

        void SaveSettings();

        void UpdateConfig();
    }
}