using StartPos.Shared.Interfaces;
using StartPos.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    [Serializable]
    [XmlRoot("Settings")]
    public class Config : IConfig
    {
        public bool IsStartPosEnabled { get; set; } = true;
        public string AppToRun { get; set; } = "pcpos7";
        public Contractor Contractor { get; set; } = Contractor.Default();
        public int PosNumber { get; set; }
        public bool IsClientMonitorInstalled { get; set; }
        public bool IsInsoftUpdateServerInstalled { get; set; }
        public bool IsAllDayShop { get; set; }
        public Database Database { get; set; } = Database.Default();
        public ObligatoryBackup ObligatoryBackup { get; set; } = ObligatoryBackup.Default();
        public AlternativeBackup AlternativeBackup { get; set; } = AlternativeBackup.Default();
        public CscApiClientConfig ApiClientConfig { get; set; }
        public UpdaterConfig Update { get; set; }

        [XmlArray("DataBackupItems")]
        [XmlArrayItem("DataItem")]
        public List<DataItem> DataBackupItems { get; set; }

        public void SaveSettings()
        {
            XmlHelpers.Save(this, Constants.ConfigFile);
        }

        public void UpdateConfig()
        {
            DataBackupItems = (DataBackupItems?.Count == 0 || DataBackupItems == null) ? DataItem.Default() : DataBackupItems;
            ApiClientConfig = ApiClientConfig ?? CscApiClientConfig.Default();
            Update = Update ?? UpdaterConfig.Default();
        }
    }
}