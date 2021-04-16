using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class ObligatoryBackup
    {
        [XmlAttribute]
        public bool Active { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        public static ObligatoryBackup Default()
        {
            return new ObligatoryBackup
            {
                Active = true,
                Path = @"C:\Pcmwin\StartPos_Backup",
            };
        }
    }
}