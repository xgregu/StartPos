using StartPos.Shared.Enums;
using StartPos.Shared.Utils;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class DataItem
    {
        private const string PcPosRegistryKey = @"SOFTWARE\Insoft sp. z o.o.\PC-POS 7";
        private const string PcPosLocationRegistryKeyValue = "Location";

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public BackupItemType Type { get; set; }

        public static List<DataItem> Default()
        {
            string instalationDir = RegistryOperations.GetValueRegistry(PcPosRegistryKey, PcPosLocationRegistryKeyValue);
            var defaultBackupFileList = new List<DataItem>
            {
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "pcpos7.conf"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "pcpos7.jar"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "launcher.conf"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "receiptViewer.conf"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "launcher.jar"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "receipticons.zip"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "serial.enc"), Type = BackupItemType.File },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "cardPanels"), Type = BackupItemType.Directory },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "displaysystem"), Type = BackupItemType.Directory },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "labels"), Type = BackupItemType.Directory },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "printFormats"), Type = BackupItemType.Directory },
                new DataItem { Path = System.IO.Path.Combine(instalationDir, "billbird"), Type = BackupItemType.Directory }
            };

            return defaultBackupFileList;
        }
    }
}