using System;

namespace Csc.ApiClient.Models
{
    public class Report
    {
        public string Service { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public string Localization { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Fatal { get; set; } = string.Empty;
        public string Warrning { get; set; } = string.Empty;
        public string ReportContent { get; set; } = string.Empty;
        public string Suplement { get; set; } = string.Empty;
    }
}