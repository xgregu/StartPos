using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class UpdaterConfig
    {
        [XmlElement]
        public string VersionInfoUrl { get; set; }

        [XmlElement]
        public bool IsAutoUpdate { get; set; }

        public static UpdaterConfig Default() =>
            new UpdaterConfig
            {
                VersionInfoUrl = @"http://ts.coma.tychy.pl/download/startpos/startpos.xml",
                IsAutoUpdate = true
            };
    }
}