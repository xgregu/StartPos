using NLog;
using StartPos.Enums;
using StartPos.Forms;
using StartPos.Interfaces;
using StartPos.Services;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartPos.Flows
{
    internal class RestartFlow : IFlow
    {
        private readonly IBackupService _backupService;
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly IFirewallService _firewallService;
        private readonly List<string> _listQueryTableBackup;
        private readonly ILogger _logger;
        private readonly IPcPosService _pcPosService;
        private readonly ISqlDatabaseService _sqlDatabaseService;
        private readonly ISystemService _systemService;
        private readonly ILogger _windowLogger;

        public RestartFlow(IContext context, IBackupService backupService, ISqlDatabaseService sqlDatabaseService, IFirewallService firewallService, ISystemService systemService, IConfig config, IPcPosService pcPosService)
        {
            _pcPosService = pcPosService;
            _context = context;
            _backupService = backupService;
            _sqlDatabaseService = sqlDatabaseService;
            _firewallService = firewallService;
            _systemService = systemService;
            _config = config;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);

            _listQueryTableBackup = new List<string>
            {
                SqlQuerys.SelectCustomerCardFormatTable,
                SqlQuerys.SelectOperatorTable,
                SqlQuerys.SelectPosDocCounterTable,
                SqlQuerys.SelectProfileConfigTable
            };
        }

        public string Name { get; } = Constants.Flow.Restart;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.Silent;

        public async Task Run()
        {
            await Task.Run(FlowTask);
        }

        private async Task FlowTask()
        {
            if (!_config.IsStartPosEnabled)
            {
                _windowLogger.Warn("Mechanizmy oprogramowania StartPos są wyłączone.");
                _logger.Warn($"{nameof(FlowTask)} | StartPos mechanisms are disabled");
                if (Equals(_context.ActiveFlow, Constants.Flow.Restart))
                    await RestartProcedure();
                else
                    await ShutdownProcedure();

                return;
            }

            _windowLogger.Info("Uruchomienie modułu restartu StartPos");

            if (Equals(_context.ActiveFlow, Constants.Flow.Auto) && _config.IsAllDayShop &&
                !_config.Contractor.IsContract)
            {
                _logger.Info($"{nameof(FlowTask)} | The conditions for restarting are not met");
                return;
            }

            _pcPosService.KillPcPos();

            await Task.WhenAll(
                _backupService.ObligatoryBackup(),
                _backupService.BackupPcPosDirectory(),
                _firewallService.AddFirewallRule());

            _sqlDatabaseService.CheckIntegrity();
            _sqlDatabaseService.BackupDatabase();

            if (_config.Contractor.IsContract)
                _sqlDatabaseService.Defragmentation();

            _listQueryTableBackup.ForEach(i => _sqlDatabaseService.BackupTable(i));

            await _backupService.AlternativeBackup();

            _systemService.ClearTempDirectory();

            if (Equals(_context.ActiveFlow, Constants.Flow.Restart) || Equals(_context.ActiveFlow, Constants.Flow.Auto))
                await RestartProcedure();
            else
                await ShutdownProcedure();
        }

        private Task RestartProcedure()
        {
            _windowLogger.Info("Rozpoczęcie procedury restartu systemu...");
            using (var countdownForm = new CountdownForm())
            {
                if (countdownForm.ShowDialog() == DialogResult.OK)
                {
                    _windowLogger.Info("System zostanie uruchomiony ponownie...");
                    _systemService.Restart();
                }
                else
                {
                    _windowLogger.Info("Przerwano procedure restartu systemu...");
                }
            }

            return Task.CompletedTask;
        }

        private Task ShutdownProcedure()
        {
            _windowLogger.Info("Rozpoczęcie procedury zamknięcia systemu...");
            using (var countdownForm = new CountdownForm())
            {
                if (countdownForm.ShowDialog() == DialogResult.OK)
                {
                    _windowLogger.Info("System zostanie zamknięty...");
                    _systemService.Shutdown();
                }
                else
                {
                    _windowLogger.Info("Przerwano procedure zamknięcia...");
                }
            }

            return Task.CompletedTask;
        }
    }
}