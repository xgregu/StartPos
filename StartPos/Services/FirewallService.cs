using NLog;
using StartPos.Interfaces;
using StartPos.Shared;
using System;
using System.Collections;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace StartPos.Services
{
    internal class FirewallService : IFirewallService
    {
        private readonly IContext _context;
        private readonly bool _isFirewallRuleExist;
        private readonly ILogger _logger;
        private readonly string _ruleDisplayName;
        private readonly ILogger _windowLogger;

        public FirewallService(IContext context)
        {
            _context = context;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _ruleDisplayName = $"COMA-StartPos-{_context.PcPosInfo.Port}";
            _isFirewallRuleExist = IsFirewallRuleExist();
        }

        public Task AddFirewallRule()
        {
            if (_isFirewallRuleExist)
                return Task.CompletedTask;

            try
            {
                _windowLogger.Info("Konfiguracja zapory Windows");
                _logger.Info($"{nameof(AddFirewallRule)} | Windows firewall configuration...");
                var powershell = PowerShell.Create();
                var psCommand =
                    $"New-NetFirewallRule -DisplayName \"{_ruleDisplayName}\" -Direction Inbound -LocalPort {_context.PcPosInfo.Port} -Protocol TCP -Action Allow";
                powershell.Commands.AddScript(psCommand);
                powershell.Invoke();
            }
            catch (Exception ex)
            {
                _windowLogger.Warn("Błąd podczas dodawania reguły w zaporze Windows");
                _logger.Error(ex, $"{nameof(AddFirewallRule)} | Error adding rule in firewall.");
            }
            return Task.CompletedTask;
        }

        private bool IsFirewallRuleExist()
        {
            var tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            dynamic fwPolicy2 = Activator.CreateInstance(tNetFwPolicy2);
            var rules = fwPolicy2.Rules as IEnumerable;
            return rules?.Cast<dynamic>().Any(rule => rule.Name == _ruleDisplayName) ?? false;
        }
    }
}