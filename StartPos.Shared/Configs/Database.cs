using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class Database
    {
        [XmlAttribute]
        public string Instance { get; set; } //InstancjaSQL

        [XmlAttribute]
        public string Host { get; set; }

        [XmlAttribute]
        public string Username { get; set; }

        [XmlAttribute]
        public string Password { get; set; } //HasloSQL

        public static Database Default()
        {
            return new Database
            {
                Instance = "SQLEXPRESS",
                Host = "localhost",
                Username = "sa",
                Password = "10coma123",
            };
        }
    }
}