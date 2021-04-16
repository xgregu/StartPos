using NLog;
using StartPos.Shared.Extesions;
using StartPos.Shared.Interfaces;
using StartPos.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StartPos.Shared.Services
{
    public class SystemService : ISystemService
    {
        private readonly List<string> _disableSleepComannds;

        private readonly ILogger _logger;

        public SystemService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _disableSleepComannds = PrepareDisableSleepCommands();
        }

        public void Restart()
        {
            _logger.Info($"{nameof(Restart)} | System restart...");
            AppsOperations.Start(@"shutdown.exe", "-r -t 0");
        }

        public void Shutdown()
        {
            _logger.Info($"{nameof(Shutdown)} | System shutdown...");
            AppsOperations.Start(@"shutdown.exe", " -s -f -t 0");
        }

        public Task DisableSystemSleep()
        {
            _disableSleepComannds.ForEach(x => AppsOperations.Start("cmd.exe", $"/c {x}", ProcessWindowStyle.Hidden, true));
            return Task.CompletedTask;
        }

        private static List<string> PrepareDisableSleepCommands()
        {
            return new List<string>
            {
                @"powercfg -h off",
                @"sc stop DiagTrack",
                @"sc stop dmwappushservice",
                @"sc delete DiagTrack",
                @"sc delete dmwappushservice",
                @"echo "" > C:\ProgramData\Microsoft\Diagnosis\ETLLogs\AutoLogger\AutoLogger -Diagtrack -Listener.etl",
                @"powercfg -SETACTIVE 381b4222 -f694 -41f0 -9685 -ff5bb260df2e",
                @"powercfg.exe -change -monitor-timeout-dc 360",
                @"powercfg.exe -change -standby-timeout-dc 0",
                @"powercfg.exe -change -hibernate-timeout-dc 0",
                @"powercfg.exe -change -disk-timeout-dc 360",
                @"powercfg.exe -change -monitor-timeout-ac 360",
                @"powercfg.exe -change -standby-timeout-ac 0",
                @"powercfg.exe -change -hibernate-timeout-ac 0",
                @"powercfg.exe -change -disk-timeout-ac 360",
                @"powercfg -SETDCVALUEINDEX 381b4222 -f694 -41f0 -9685 -ff5bb260df2e 7516b95f -f776 -4464 -8c53 -06167f40cc99 17aaa29b -8b43 -4b94 -aafe -35f64daaf1ee 0",
                @"powercfg -SETACVALUEIDEX 381b4222 -f694 -41f0 -9685 -ff5bb260df2e 7516b95f -f776 -4464 -8c53 -06167f40cc99 17aaa29b -8b43 -4b94 -aafe -35f64daaf1ee 0",
                @"net config server /autodisconnect:-1"
            };
        }

        public DateTime SystemUpTime()
        {
            using (var uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue();
                return DateTime.Now - TimeSpan.FromSeconds(uptime.NextValue());
            }
        }

        public long GetDirectorySizeMb(string path)
        {
            if (!Directory.Exists(path))
                return -1;

            long dirSize = 0;
            new DirectoryInfo(path).GetFiles("*", SearchOption.AllDirectories).ToList().ForEach(x => dirSize += x.Length);
            return dirSize.ConvertBytesToMegabytes();
        }

        public long GetFreeDiskSpaceMb(string path)
        {
            DriveInfo dDrive = null;
            var directoryInfo = new DirectoryInfo(path);
            try
            {
                dDrive = new DriveInfo(directoryInfo.Root.FullName);
            }
            catch
            {
                return -1;
            }

            if (!dDrive.IsReady)
                return -1;

            return dDrive.AvailableFreeSpace.ConvertBytesToMegabytes();
        }

        public void ClearTempDirectory()
        {
            _logger.Info($"{nameof(ClearTempDirectory)} | Cleaning up temporary files...");
            var tempDirs = new List<string>
            {
                Environment.GetEnvironmentVariable("temp"),
                Environment.GetEnvironmentVariable("tmp"),
                Path.Combine(Environment.GetEnvironmentVariable("windir"), "temp"),
                Path.GetTempPath()
            };

            tempDirs.ForEach(DeleteDirectory);
        }

        private static void DeleteDirectory(string path)
        {
            var di = new DirectoryInfo(path);
            if (!di.Exists)
                return;

            try
            {
                foreach (var file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            catch { }
            try
            {
                foreach (var dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch { }
        }
    }
}