using NLog;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Utils;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StartPos.Services
{
    internal class CronService : ICronService
    {
        private const string CronTabX86File = @"C:\Program Files (x86)\cron\cron.tab";
        private const string CronTabFile = @"C:\Program Files\cron\cron.tab";
        private const string CronExeX86File = @"C:\Program Files (x86)\cron\cron.exe";
        private const string CronExeFile = @"C:\Program Files\cron\cron.exe";

        private readonly ILogger _logger;
        private readonly ILogger _windowLogger;

        public CronService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
        }

        public Task InstallCron()
        {
            if (IsCronInstaled())
            {
                _windowLogger.Info("nCron - zainstalowany");
                _logger.Info($"{nameof(InstallCron)} | nCron - installed");
                return Task.CompletedTask;
            }

            _windowLogger.Info("Instalacja nCron...");
            _logger.Info($"{nameof(InstallCron)} | Installing nCron...");
            AppsOperations.StartAndWait(Path.Combine(Constants.BaseDir, @"Installers\ncron.exe"), "");
            return Task.CompletedTask;
        }

        public Task ConfigureCron()
        {
            if (!IsCronInstaled())
                return Task.CompletedTask;

            _windowLogger.Info($"{nameof(ConfigureCron)} | Konfiguracja nCron...");
            _logger.Info($"{nameof(ConfigureCron)} | NCron configuration");
            var cronTabFile = GetCronTabFilePath();
            try
            {
                var newValue = $"00 01 * * * \"{Path.Combine(Constants.BaseDir, "StartPos.exe")} auto\"";
                var cronTabLines = File.ReadAllLines(cronTabFile).ToList();
                var lineIndex = cronTabLines.FindIndex(x => x.ToLower().Contains("startpos.exe"));

                if (lineIndex >= 0)
                    cronTabLines.RemoveAt(lineIndex);
                cronTabLines.Add(newValue);
                File.WriteAllLines(cronTabFile, cronTabLines);
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Błąd podczas konfiguracji nCron. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(ConfigureCron)} | Error configuration nnCrone.");
            }
            finally
            {
                _windowLogger.Info("...zakończone.");
            }

            return Task.CompletedTask;
        }

        public Task DisableCron()
        {
            if (!IsCronInstaled())
                return Task.CompletedTask;

            _windowLogger.Info("Wyłączenie nCron...");
            _logger.Error($"{nameof(ConfigureCron)} | Disabling nCron...");
            try
            {
                AppsOperations.StartAndWait(GetCronExeFilePath(), "-q -remove");

                if (File.Exists(GetCronTabFilePath()))
                    File.Delete(GetCronTabFilePath());
            }
            catch (Exception ex)
            {
                _windowLogger.Error("Błąd podczas wyłączania nCron. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(DisableCron)} | Error disable nnCrone.");
            }

            return Task.CompletedTask;
        }

        private static bool IsCronInstaled()
        {
            return File.Exists(CronTabX86File) || File.Exists(CronTabFile);
        }

        private static string GetCronTabFilePath()
        {
            return File.Exists(CronTabX86File) ? CronTabX86File : CronTabFile;
        }

        private static string GetCronExeFilePath()
        {
            return File.Exists(CronExeX86File) ? CronExeX86File : CronExeFile;
        }
    }
}