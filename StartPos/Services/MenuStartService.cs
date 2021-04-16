using NLog;
using StartPos.Interfaces;
using StartPos.Setup;
using StartPos.Shared;
using StartPos.Shared.Utils;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StartPos.Services
{
    internal class MenuStartService : IMenuStartService
    {
        private const string ClassicShellExeX86File = @"C:\Program Files (x86)\Classic Shell\ClassicStartMenu.exe";
        private const string ClassicShellExeFile = @"C:\Program Files\Classic Shell\ClassicStartMenu.exe";

        private readonly ILogger _windowLogger;
        private readonly ILogger _logger;

        private readonly string _xmlSettingFile =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Installers\ClassicShellSettings.xml");

        public MenuStartService()
        {
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _logger = LogManager.GetCurrentClassLogger();
        }

        public Task InstallClassicShell()
        {
            if (IsClassicShellInstaled())
            {
                _windowLogger.Info("ClassicStartMenu - zainstalowane");
                _logger.Info($"{nameof(InstallClassicShell)} | ClassicStartMenu - installed");
                return Task.CompletedTask;
            }

            _windowLogger.Info("Instalacja ClassicStartMenu...");
            _logger.Info($"{nameof(InstallClassicShell)} | Installing ClassicStartMenu...");
            AppsOperations.Start(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Installers\ClassicShellSetup.exe"), "/qn");
            return Task.CompletedTask;
        }

        public Task ConfigureClassicShell()
        {
            if (!IsClassicShellInstaled())
                return Task.CompletedTask;

            _windowLogger.Info("Konfiguracja ClassicStartMenu...");
            _logger.Info($"{nameof(ConfigureClassicShell)} | ClassicStartMenu configuration");
            PrepareXmlSettingFile();
            AppsOperations.Start(GetClassicShellExeFilePath(), "-xml " + _xmlSettingFile, System.Diagnostics.ProcessWindowStyle.Hidden, true);
            return Task.CompletedTask;
        }

        private static bool IsClassicShellInstaled() => File.Exists(ClassicShellExeX86File) || File.Exists(ClassicShellExeFile);

        private static string GetClassicShellExeFilePath() => File.Exists(ClassicShellExeX86File) ? ClassicShellExeX86File : ClassicShellExeFile;

        private void PrepareXmlSettingFile()
        {
            var bodyXml = string.Format(MenuStartXmlSettingFile.XmlBody,
                Path.Combine(Constants.BaseDir, "StartPos.exe"), Path.Combine(Constants.BaseDir, "Icons"));
            if (File.Exists(_xmlSettingFile))
                File.Delete(_xmlSettingFile);

            File.WriteAllText(_xmlSettingFile, bodyXml);
        }
    }
}