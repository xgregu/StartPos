using Csc.ApiClient;
using System.Xml.Serialization;

namespace StartPos.Shared.Configs
{
    public class CscApiClientConfig : ICscApiClientConfig
    {
        [XmlElement]
        public string CscApiUrl { get; set; }

        [XmlElement]
        public bool StoreUnsendedReports { get; set; }

        public static CscApiClientConfig Default() =>
            new CscApiClientConfig
            {
                CscApiUrl = "http://api.ts.coma.tychy.pl/api/v1",
                StoreUnsendedReports = true
            };
    }
}