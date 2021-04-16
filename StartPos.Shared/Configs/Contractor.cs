using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class Contractor
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsContract { get; set; }

        [XmlAttribute]
        public string LocationName { get; set; }

        public static Contractor Default()
        {
            return new Contractor
            {
                Name = string.Empty,
                IsContract = false,
                LocationName = string.Empty,
            };
        }
    }
}