using System;
using System.IO;

namespace StartPos.Shared
{
    public static class Constants
    {
        public static readonly string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        public const string CleanAppName = "StartPos";
        public const string WindowLoggerName = "windowLog";
        public const string ConfigDir = "Config";
        public static readonly string ConfigFile = Path.Combine(BaseDir, ConfigDir, "Config.xml");

        public static class Flow
        {
            public const string Main = "MAIN";
            public const string Diagnostic = "DIAGNOSTIC";
            public const string Repair = "REPAIR";
            public const string Restart = "RESTART";
            public const string Auto = "AUTO";
            public const string Shutdown = "SHUTDOWN";
            public const string Disable = "DISABLE";
            public const string Setup = "SETUP";
            public const string SetupSilent = "RESTART_CRONE";
            public const string Test = "TEST";
            public const string BuildInstaller = "BUILD_INSTALLER";
        }
    }
}