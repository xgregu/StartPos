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
    internal class RepairFlow : IFlow
    {
        private readonly IBackupService _backupService;
        private readonly IConfig _config;
        private readonly IFirewallService _firewallService;
        private readonly List<string> _listQueryTableBackup;
        private readonly IPcPosService _pcPosService;
        private readonly ISqlDatabaseService _sqlDatabaseService;
        private readonly ISystemService _systemService;
        private readonly ILogger _windowLogger;
        private readonly ILogger _logger;

        public RepairFlow(IConfig config, IBackupService backupService, ISqlDatabaseService sqlDatabaseService,
            IFirewallService firewallService, ISystemService systemService, IPcPosService pcPosService)
        {
            _pcPosService = pcPosService;
            _backupService = backupService;
            _sqlDatabaseService = sqlDatabaseService;
            _firewallService = firewallService;
            _systemService = systemService;
            _config = config;

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

        public string Name { get; } = Constants.Flow.Repair;
        public UpdaterMode UpdateMode { get; } = UpdaterMode.Normal;

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
                return;
            }

            using (var passwordForm = new PasswordForm())
            {
                if (passwordForm.ShowDialog() != DialogResult.OK)
                    return;
            }

            _pcPosService.KillPcPos();

            await Task.WhenAll(_backupService.ObligatoryBackup(),
                _backupService.BackupPcPosDirectory(),
                _firewallService.AddFirewallRule());

            await _backupService.AlternativeBackup();

            _sqlDatabaseService.CheckIntegrity();
            _sqlDatabaseService.BackupDatabase();
            _sqlDatabaseService.Defragmentation();

            _listQueryTableBackup.ForEach(i => _sqlDatabaseService.BackupTable(i));

            await _backupService.AlternativeBackup();

            _systemService.ClearTempDirectory();
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
        }
    }
}