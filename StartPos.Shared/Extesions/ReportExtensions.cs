using Csc.ApiClient.Models;
using System;

namespace StartPos.Shared.Extesions
{
    public static class ReportExtensions
    {
        public static Report CreateEmpty(this Report report, string customer, string localization, string appVersion, string service)
        {
            report.Customer = customer;
            report.Localization = localization;
            report.Service = service;
            report.Version = appVersion;
            report.Timestamp = DateTime.Now;
            return report;
        }
    }
}