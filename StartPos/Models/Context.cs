using Csc.ApiClient.Models;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Extesions;
using StartPos.Shared.Interfaces;
using System.IO;
using System.Reflection;

namespace StartPos.Models
{
    public class Context : IContext
    {
        public Context(IPcPosInfo pcPosInfo, IConfig config)
        {
            AppVersion = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
            PcPosInfo = pcPosInfo;
            TempPath = GetTempPath();
            TsReport = new Report().CreateEmpty(config.Contractor.Name, config.Contractor.LocationName, AppVersion, Constants.CleanAppName);
        }

        public string AppVersion { get; }
        public string TempPath { get; }
        public IPcPosInfo PcPosInfo { get; }
        public string ActiveFlow { get; set; }
        public Report TsReport { get; set; }

        private static string GetTempPath()
        {
            var tempPath = Path.GetTempPath();
            return Directory.Exists(tempPath)
                ? Path.Combine(tempPath, Constants.CleanAppName)
                : $@"C:\tmp\{Constants.CleanAppName}\";
        }
    }
}