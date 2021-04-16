using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class AlternativeBackup
    {
        [XmlAttribute]
        public bool Active { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        public static AlternativeBackup Default()
        {
            return new AlternativeBackup
            {
                Active = false,
                Path = string.Empty,
            };
        }
    }
}