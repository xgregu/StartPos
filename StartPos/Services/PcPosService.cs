using NLog;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using StartPos.Shared.Utils;
using StartPos.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace StartPos.Services
{
    internal class PcPosService : IPcPosService
    {
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly ILogger _logger;
        private readonly ILogger _windowLogger;

        public PcPosService(IConfig config, IContext context)
        {
            _config = config;
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
        }

        public async Task StartDiagnostic()
        {
            var startArgs = $"-Xmx800M -jar \"{_context.PcPosInfo.InstalationDir}\\pcpos7.jar\" .\\pcpos7.conf localconf /path=\"{_context.PcPosInfo.InstalationDir}\"";

            _windowLogger.Info("Uruchamianie Diagnostyki Pc-Pos ...");
            _logger.Info($"{nameof(StartDiagnostic)} | Start Pc-Pos. {_context.PcPosInfo.JavaPath} {startArgs}");
            if (!AppsOperations.StartAndWait(_context.PcPosInfo.JavaPath, startArgs, _context.PcPosInfo.InstalationDir, ProcessWindowStyle.Minimized))
            {
                _windowLogger.Error("Błąd podczas uruchamiania aplikacji Diagnostyka Pc-Pos. Szczegóły w log.");
                _logger.Error($"{nameof(StartDiagnostic)} | Error during startup diagnostic Pc-Pos");
                await Task.Delay(5000);
            }
        }

        public async Task StartPcPos()
        {
            if (PcPosIsRunning())
            {
                _windowLogger.Warn(
                    $"Aplikacja Pc-Pos jest już uruchomiona. Port {_context.PcPosInfo.MutexPort} jest zablokowany.");
                _logger.Warn($"{nameof(StartPcPos)} | Pc-Pos is running. Port {_context.PcPosInfo.MutexPort} is blocked.");
                await Task.Delay(5000);
                return;
            }
            var startArgs = _config.IsInsoftUpdateServerInstalled
                    ? $"-Xmx800M -jar \"{_context.PcPosInfo.InstalationDir}\\launcher.jar\" .\\launcher.conf"
                    : $"-Xmx800M -jar \"{_context.PcPosInfo.InstalationDir}\\pcpos7.jar\" .\\pcpos7.conf /path=\"{_context.PcPosInfo.InstalationDir}\"";

            _windowLogger.Info("Uruchamianie programu kasowego...");
            _logger.Info($"{nameof(StartPcPos)} | Start Pc-Pos. {_context.PcPosInfo.JavaPath} {startArgs}");
            if (!AppsOperations.StartAndWait(_context.PcPosInfo.JavaPath, startArgs, _context.PcPosInfo.InstalationDir, ProcessWindowStyle.Minimized))
            {
                _windowLogger.Error("Błąd podczas uruchamiania aplikacji Pc-Pos. Szczegóły w log.");
                _logger.Error($"{nameof(StartPcPos)} | Error during startup Pc-Pos");
                await Task.Delay(5000);
                return;
            }
            await Task.Delay(5000);
            Task.WaitAll(
                StartReciptViewer(),
                ControlCorrectnessWorkPcPos()
            );
        }

        public void UpdateInsoftUpdateFiles()
        {
            UpdatePcPosConfig("SoftwareUpdate");
            UpdatePcPosLauncher();
        }

        public void ClientMonitorUpdateFiles() => UpdatePcPosConfig("CashierEvents");

        public void KillPcPos() => Process.GetProcesses()
                .Where(x => x.ProcessName.ToLower().Contains("java"))
                .ToList()
                .ForEach(x => x.Kill());

        private bool PcPosIsRunning()
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();
            return tcpConnInfoArray.Any(endpoint => endpoint.Port == _context.PcPosInfo.MutexPort);
        }

        private async Task ControlCorrectnessWorkPcPos()
        {
            int waitSec = 60;
            if (PcPosIsRunning())
            {
                _windowLogger.Info("Aplikacja Pc-Pos działa");
                _logger.Info("Pc-Pos is running");
                _context.TsReport.Message += $" ► Aplikacja Pc-Pos uruchomiona bez błędów{Environment.NewLine}";
                return;
            }

            _windowLogger.Info($"Pc-Pos nie jest uruchomiony, czekam {waitSec}s...");
            _logger.Info($"{nameof(ControlCorrectnessWorkPcPos)} | Pc-Pos no running, I'm waiting...");
            await Watch.Wait(() => { },
                () => PcPosIsRunning(), waitSec / 2);

            if (PcPosIsRunning())
            {
                _windowLogger.Info("Aplikacja Pc-Pos działa");
                _logger.Info("Pc-Pos is running");
                _context.TsReport.Message += $" ► Aplikacja Pc-Pos uruchomiona bez błędów{Environment.NewLine}";
                return;
            }

            if (_config.IsInsoftUpdateServerInstalled)
            {
                _windowLogger.Warn(
                    $"PC-POS wraz SERWEREM AKTUALIZACJI został uruchomiony bez błędów, ale wygląda na to że nie działa. Port {_context.PcPosInfo.MutexPort} jest wolny. Próba uruchomienia PC-POS bez SERWERA AKTUALIZACJI.");
                _logger.Warn($"{nameof(ControlCorrectnessWorkPcPos)} | PC-POS with UPDATE SERVER was started without errors, but does not appear to be working. {_context.PcPosInfo.MutexPort} port is open. Attempted to start PC-POS without UPDATE SERVER.");
                _context.TsReport.Warrning += $" ► PC-POS wraz SERWEREM AKTUALIZACJI został uruchomiony bez błędów, ale wygląda na to że nie działa. Port {_context.PcPosInfo.MutexPort} jest wolny. Próba uruchomienia PC-POS bez SERWERA AKTUALIZACJI.{Environment.NewLine}";
                _config.IsInsoftUpdateServerInstalled = false;
                await StartPcPos();
                return;
            }

            _windowLogger.Error(
                $"PC-POS został uruchomiony bez błędów, ale wygląda na to że PC-POS nie działa. Port {_context.PcPosInfo.MutexPort} jest wolny.");
            _logger.Error($"{nameof(ControlCorrectnessWorkPcPos)} | PC-POS with UPDATE SERVER was started without errors, but does not appear to be working. {_context.PcPosInfo.MutexPort} port is open.");
            _context.TsReport.Fatal += $" ► PC-POS został uruchomiony bez błędów, ale wygląda na to że nie działa. Port {_context.PcPosInfo.MutexPort} jest wolny.{Environment.NewLine}";
        }

        private async Task StartReciptViewer()
        {
            if (!_config.IsClientMonitorInstalled)
                return;

            Process.GetProcesses()
                .Where(x => x.MainWindowTitle.ToLower().Contains("ekran klienta"))
                .ToList()
                .ForEach(x => x.Kill());

            var startArgs = $"-cp \"{_context.PcPosInfo.InstalationDir}\\pcpos7.jar\" pl.com.insoft.receiptviewer.ReceiptViewerMainEntry \"{_context.PcPosInfo.InstalationDir}\\receiptViewer.conf\"";

            _windowLogger.Info("Uruchamianie monitora klienta...");
            _logger.Info($"{nameof(StartReciptViewer)} | Start Recipt Viewer. {_context.PcPosInfo.JavaPath} {startArgs}");
            if (!AppsOperations.StartAndWait(_context.PcPosInfo.JavaPath, startArgs, _context.PcPosInfo.InstalationDir, ProcessWindowStyle.Minimized))
            {
                _windowLogger.Error("Błąd podczas uruchamiania aplikacji Monitor Klienta. Szczegóły w log.");
                _context.TsReport.Warrning += $" ► Błąd podczas uruchamiania aplikacji Monitor Klienta.{Environment.NewLine}";
                _logger.Error($"{nameof(StartReciptViewer)} | Error during startup Recipt Viewer");
                await Task.Delay(5000);
            }
        }

        private void UpdatePcPosConfig(string section)
        {
            Dictionary<string, string> parameters = null;

            if (section.Equals("SoftwareUpdate"))
                parameters = GetParametersSoftwareUpdate();

            if (section.Equals("CashierEvents"))
                parameters = GetParametersCashierEvents();

            if (parameters == null)
                return;

            string confBodyLine;
            var isFindSectionSoftwareUpdate = false;
            var confFile = Path.Combine(_context.PcPosInfo.InstalationDir, $"{_config.AppToRun}.conf");

            _logger.Info($"{nameof(UpdatePcPosConfig)} | Updating PcPos Config {confFile}. Section: {section}");

            var confFileTmp = $"{confFile}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.bak";

            DeleteOldBakfFile("pcpos7.conf");
            CreateConfigFileBackup(confFile, confFileTmp);

            using (var bodyPcPosConfig = File.OpenText(confFileTmp))
            {
                while ((confBodyLine = bodyPcPosConfig.ReadLine()) != null)
                {
                    confBodyLine = confBodyLine.Trim('\t');

                    if (confBodyLine.ToLower().Equals($"section: {parameters.Values.First().ToLower()}"))
                    {
                        isFindSectionSoftwareUpdate = true;
                        continue;
                    }

                    if (!isFindSectionSoftwareUpdate)
                        continue;

                    var parameterKey = parameters.Keys.FirstOrDefault(x =>
                        x.ToLower().Contains(confBodyLine.ToLower().Split(':').FirstOrDefault() ?? string.Empty));

                    if (!string.IsNullOrEmpty(parameterKey) && confBodyLine.ToLower().Contains(parameterKey.ToLower()))
                    {
                        try
                        {
                            if (!parameters.ContainsKey(parameterKey))
                                continue;

                            if (parameters[parameterKey].Equals("skip_parametr"))
                                continue;

                            var parametrValue = parameters[parameterKey];

                            var bodyConfAllText = File.ReadAllText(confFile);

                            var newLineValue = $"\t{parameterKey}: {parametrValue}";

                            if (confBodyLine == newLineValue)
                                continue;
                            _windowLogger.Info($"Aktualizacja {confFile}: {confBodyLine.Trim('\t')} => {newLineValue}");
                            _logger.Info($"{nameof(UpdatePcPosConfig)} | Update parametr in {confFile}: {confBodyLine.Trim('\t')} => {newLineValue}");
                            bodyConfAllText = bodyConfAllText.Replace(confBodyLine, newLineValue.Trim('\t'));
                            File.WriteAllText(confFile, bodyConfAllText);
                        }
                        catch (Exception ex)
                        {
                            _windowLogger.Error($"Błąd podczas aktualizacji {confFile}. Szczegóły w log.");
                            _logger.Error(ex, $"{nameof(UpdatePcPosConfig)} | Error update {confFile}");
                        }
                    }
                    else
                    {
                        _windowLogger.Warn(
                            $"Błąd podczas aktualizacji {confFile}. Problem z parametrem. Sprawdź poprawność sekcji {section}!");
                        _logger.Error(
                            $"{nameof(UpdatePcPosConfig)} | Error update {confFile}. Parameter problem. Check if the {section} section is correct!");
                    }

                    if (confBodyLine.ToLower().Equals($"endsection: {parameters.Values.Last().ToLower()}"))
                        break;
                }
            }

            if (!isFindSectionSoftwareUpdate)
                parameters.ToList().ForEach(x =>
                    File.AppendAllText(confFile, $"\t{x.Key}: {x.Value}{Environment.NewLine}"));
        }

        private void DeleteOldBakfFile(string fileName)
        {
            var filesToDelete = new DirectoryInfo(_context.PcPosInfo.InstalationDir).GetFiles("*.bak").Where(p => p.Name.ToLower().Contains(fileName)).ToList();

            try
            {
                filesToDelete.ForEach(x => File.Delete(x.FullName));
            }
            catch (Exception e)
            {
                _logger.Error($"{nameof(DeleteOldBakfFile)} | File delete errror: {e.Message}");
            }
        }

        private static void CreateConfigFileBackup(string confFile, string confFileTmp) => File.Copy(confFile, confFileTmp, true);

        private Dictionary<string, string> GetParametersSoftwareUpdate() => new Dictionary<string, string>
            {
                {"Section", "SoftwareUpdate"},
                {"RemoteUrl", "pcpos7.jar"},
                {"LocalDir", $"${Path.Combine(_context.PcPosInfo.InstalationDir, "upgradesklep")}"},
                {"OnlyUpgradeAllowed", "N"},
                {"CloseAppAfterDownload", "N"},
                {"CheckInterval", "5"},
                {"EndSection", "SoftwareUpdate"}
            };

        private Dictionary<string, string> GetParametersCashierEvents() => new Dictionary<string, string>
            {
                {"Section", "CashierEvents"},
                {"Serial", "skip_parametr"},
                {"TCPIP", "Y"},
                {"SerialConnectString", "skip_parametr"},
                {"FullReceipt", "Y"},
                {"TCPIPHost", "127.0.0.1"},
                {"TCPIPPort", "1002"},
                {"TCPIPRetryTimeoutMs", "10000"},
                {"CharSet", "skip_parametr"},
                {"SendBarcode", "skip_parametr"},
                {"HTTP", "skip_parametr"},
                {"HttpHost", "skip_parametr"},
                {"HttpConnectionTimeoutMs", "skip_parametr"},
                {"HttpReadTimeoutMs", "skip_parametr"},
                {"LineSeparator", "skip_parametr"},
                {"SendMessages", "skip_parametr"},
                {"EndSection", "CashierEvents"}
            };

        private void UpdatePcPosLauncher()
        {
            var launcherBody = $@"
Section: Tasks
    TaskNum: 2
    EndSection: Tasks

Section: Task0
        Class: eu.insoft.verupdate.utils.JReplace
        Method: replace
        ArgsNum: 3
        Arg0: java.lang.String, {_context.PcPosInfo.InstalationDir.Replace('\\', '/')}
        Arg1: java.lang.String, {_context.PcPosInfo.InstalationDir.Replace(@"\", @"\\")}\\upgradesklep
        Arg2: java.lang.String, pcpos7.jar
        Classpath: {_context.PcPosInfo.InstalationDir.Replace('\\', '/')}/launcher.jar
EndSection: Task0

Section: Task1
        Class:
        Method: jar
        ArgsNum: 1
        Arg0:[Ljava.lang.String;, {_context.PcPosInfo.InstalationDir.Replace('\\', '/')}/pcpos7.conf,lang = pl
        Classpath: {_context.PcPosInfo.InstalationDir.Replace('\\', '/')}/pcpos7.jar
EndSection: Task1";

            var launcherFile = Path.Combine(_context.PcPosInfo.InstalationDir, "launcher.conf");

            _logger.Info($"{nameof(UpdatePcPosLauncher)} | Updating PcPos Launcher {launcherFile}.");

            var launcherFileTmp = $"{launcherFile}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.bak";

            if (launcherBody == File.ReadAllText(launcherFile))
                return;

            DeleteOldBakfFile("launcher.conf");
            try
            {
                if (File.Exists(launcherFile))
                    File.Move(launcherFile, launcherFileTmp);

                File.Create(launcherFile).Close();

                _windowLogger.Info($"Aktualizacja {launcherFile}.");
                _logger.Info($"{nameof(UpdatePcPosLauncher)} | Update {launcherFile}.");

                File.AppendAllText(launcherFile, launcherBody);
            }
            catch (Exception ex)
            {
                _windowLogger.Error($"Błąd podczas aktualizacji {launcherFile}. Szczegóły w log.");
                _logger.Error(ex, $"{nameof(UpdatePcPosLauncher)} | Error update {launcherFile}");
            }
        }
    }
}