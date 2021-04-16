using Csc.ApiClient;
using NLog;
using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Services;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using StartPos.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace StartPos.Flows
{
    internal class MainFlow : IFlow
    {
        private readonly ICscApiClient _apiCLient;
        private readonly IBackupService _backupService;
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly IFirewallService _firewallService;
        private readonly List<string> _listQueryTableBackup;
        private readonly ILogger _logger;
        private readonly IPcPosService _pcPosService;
        private readonly IRemoteServerService _remoteServerService;
        private readonly ISqlDatabaseService _sqlDatabaseService;
        private readonly ISqlInstanceService _sqlInstanceService;
        private readonly ILogger _windowLogger;
        private readonly ISystemService _systemService;

        public MainFlow(IContext context, IConfig config, IBackupService backupService, IRemoteServerService remoteServerService, ISqlInstanceService sqlInstanceService, ISqlDatabaseService sqlDatabaseService, IFirewallService firewallService, IPcPosService pcPosService, ICscApiClient apiClient, ISystemService systemService)
        {
            _config = config;
            _context = context;
            _backupService = backupService;
            _remoteServerService = remoteServerService;
            _sqlInstanceService = sqlInstanceService;
            _sqlDatabaseService = sqlDatabaseService;
            _firewallService = firewallService;
            _pcPosService = pcPosService;
            _apiCLient = apiClient;
            _systemService = systemService;

            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _logger = LogManager.GetCurrentClassLogger();

            _listQueryTableBackup = new List<string>
            {
                SqlQuerys.SelectCustomerCardFormatTable,
                SqlQuerys.SelectOperatorTable,
                SqlQuerys.SelectPosDocCounterTable,
                SqlQuerys.SelectProfileConfigTable
            };
        }

        public string Name { get; } = Constants.Flow.Main;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.None;

        public async Task Run() => await Task.Run(FlowTask);

        private async Task FlowTask()
        {
            if (!_config.IsStartPosEnabled)
            {
                _windowLogger.Warn("Mechanizmy oprogramowania StartPos są wyłączone.");
                _logger.Warn($"{nameof(FlowTask)} | StartPos mechanisms are disabled");
                _context.TsReport.Warrning += $" ► Mechanizmy oprogramowania StartPos są wyłączone.{Environment.NewLine}";
                return;
            }

            _windowLogger.Info("Wersja Pc-Pos: " + _context.PcPosInfo.Version);
            _context.PcPosInfo.SerializationInfo.ForEach(_windowLogger.Info);

            await Task.WhenAll(CheckInstanceIsRunning(),
                CheckRemoteServerIsActive(),
                _backupService.ObligatoryBackup(),
                _firewallService.AddFirewallRule(),
                UpdateStartPosConfig()
            );

            await _pcPosService.StartPcPos();

            _listQueryTableBackup.ForEach(i => _sqlDatabaseService.BackupTable(i));
            await _backupService.AlternativeBackupOnlyToday();
            PrepareTsReportBasic();
            await Task.Delay(5000);
        }

        private async Task UpdateStartPosConfig()
        {
            var client = await _apiCLient.GetCustomer(_config.Contractor.Name);
            if (client.Customer == null)
                return;

            if (!Equals(_config.Contractor.IsContract, client.Customer.IsServiceContract))
            {
                _logger.Info($"{nameof(UpdateStartPosConfig)} | Update customer contract - {client.Customer.Name} - {client.Customer.IsServiceContract}");
                _config.Contractor.IsContract = client.Customer.IsServiceContract;
                _config.SaveSettings();
            }
        }

        private async Task CheckInstanceIsRunning()
        {
            _windowLogger.Info("Sprawdzam czy instancja SQL jest uruchomiona...");
            _logger.Info($"{nameof(CheckInstanceIsRunning)} | Check if the SQL instance is running...");
            if (CheckInstanceAndLog())
                return;

            if (_config.Contractor.IsContract)
            {
                _logger.Info($"{nameof(CheckInstanceIsRunning)} | Attempting to start an instance: {_config.Database.Instance}");
                await Watch.Wait(() => _sqlInstanceService.TryRunInstance(_config.Database.Instance),
                    () => _sqlInstanceService.IsInstanceRunning(_config.Database.Instance), 10);

                if (CheckInstanceAndLog())
                    return;
            }
            else
            {
                _logger.Info($"{nameof(CheckInstanceIsRunning)} | Attempting to start an instance: {_config.Database.Instance}");
                await Watch.Wait(() => { },
                    () => _sqlInstanceService.IsInstanceRunning(_config.Database.Instance), 10);

                if (CheckInstanceAndLog())
                    return;
            }

            _windowLogger.Warn("Timeout sprawdzania instancji SQL...");
            _windowLogger.Error($"Instancja {_config.Database.Instance} nie działa!");
            _logger.Error($"{nameof(CheckInstanceIsRunning)} | Instance {_config.Database.Instance} is not running");
            _context.TsReport.Fatal += $" ► Instancja { _config.Database.Instance} nie działa{Environment.NewLine}";

            bool CheckInstanceAndLog()
            {
                if (!_sqlInstanceService.IsInstanceRunning(_config.Database.Instance))
                    return false;

                _windowLogger.Info($"Instancja {_config.Database.Instance} jest uruchomina");
                _logger.Info($"{nameof(CheckInstanceIsRunning)} | Instance {_config.Database.Instance} is running");
                return true;
            }
        }

        private async Task CheckRemoteServerIsActive()
        {
            _windowLogger.Info("Sprawdzam połączenie z serwerem Pc-Market...");
            _logger.Info($"{nameof(CheckRemoteServerIsActive)} | Checking the connection to the Pc-Market server");
            if (CheckServerAndLog())
                return;

            _windowLogger.Info("Brak połączenia z serwerem Pc-Market, czekam...");
            _logger.Info($"{nameof(CheckRemoteServerIsActive)} | No connection to the Pc-Market server, I'm waiting...");
            await Watch.Wait(() => { },
                () => _remoteServerService.IsActive(_context.PcPosInfo.RemoteServerIP,
                    _context.PcPosInfo.RemoteServerPort), 10);

            if (CheckServerAndLog())
                return;

            _windowLogger.Warn("Timeout sprawdzania połączenia z serwerem Pc-Market...");
            _windowLogger.Error(
                $"Brak połączenia z serwerem {_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort}");
            _logger.Error($"{nameof(CheckRemoteServerIsActive)} | No connection to the Pc-Market server:'{_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort}'");
            _context.TsReport.Fatal += $" ► Brak połączenia z serwerem {_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort}{Environment.NewLine}";

            bool CheckServerAndLog()
            {
                if (!_remoteServerService.IsActive(_context.PcPosInfo.RemoteServerIP,
                    _context.PcPosInfo.RemoteServerPort))
                    return false;

                _windowLogger.Info(
                    $"Połączenie z serwerem {_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort} poprawne");
                _logger.Info(
                    $"{nameof(CheckRemoteServerIsActive)} | Connection to the Pc-Market server:'{_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort}' is correct");
                return true;
            }
        }

        private void PrepareTsReportBasic()
        {
            var javaInfo = FileVersionInfo.GetVersionInfo(Path.Combine(_context.PcPosInfo.InstalationDir, "Java", "jre", "bin", "java.exe"));
            _context.TsReport.ReportContent += $" ► Pełny numer wersji programu: {_context.AppVersion}{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Wersja Pc-Pos: {_context.PcPosInfo.Version}{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Java: {javaInfo.ProductName} v.{javaInfo.FileVersion}\n\n{Environment.NewLine}";

            _context.TsReport.ReportContent += $" ► Uruchomienie systemu: {_systemService.SystemUpTime()}{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Baza danych Pc-Pos: {_context.PcPosInfo.DataBase}{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Data utworzenia bazy danych: {_sqlDatabaseService.DateCreation()}{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Rozmiar bazy danych: {_sqlDatabaseService.SizeMb()} MB{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Silnik bazy danych: {_sqlDatabaseService.Version()}\n\n{Environment.NewLine}";
            _context.TsReport.ReportContent += $" ► Informacje z klucza serializacyjnego: {Environment.NewLine}";
            _context.PcPosInfo.SerializationInfo.ForEach(x => _context.TsReport.ReportContent += $" ● {x}\n");
        }
    }
}