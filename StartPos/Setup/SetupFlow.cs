using StartPos.Enums;
using StartPos.Interfaces;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using StartPos.Shared.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shortcut = StartPos.Models.Shortcut;

namespace StartPos.Setup
{
    internal class SetupFlow : IFlow
    {
        private const string _autostartRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        private readonly ICronService _cronService;
        private readonly IMenuStartService _menuStartService;
        private readonly IShortcutService _shortcutService;
        private readonly ISystemService _systemService;
        private readonly ITaskShedulerService _taskShedulerService;
        private readonly IContext _context;

        public SetupFlow(IMenuStartService menuStartService, ICronService cronService, IShortcutService shortcutService, ISystemService systemService, ITaskShedulerService taskShedulerService, IContext context)
        {
            _menuStartService = menuStartService;
            _cronService = cronService;
            _shortcutService = shortcutService;
            _systemService = systemService;
            _taskShedulerService = taskShedulerService;
            _context = context;
        }

        public string Name { get; } = Constants.Flow.Setup;

        public UpdaterMode UpdateMode => _context.ActiveFlow == Constants.Flow.SetupSilent ? UpdaterMode.Silent : UpdaterMode.Normal;

        public async Task Run() => await Task.Run(FlowTask);

        private async Task FlowTask()
        {
            await _menuStartService.InstallClassicShell();

            await Task.WhenAll(
                _menuStartService.ConfigureClassicShell(),
                _systemService.DisableSystemSleep(),
                CreateShortcuts()
            );

            _taskShedulerService.ConfigureTaskSheduler();
            if (_taskShedulerService.CheckIsExist(TaskSchedulerType.AutoStart))
                RegistryOperations.DeleteValueRegistry(_autostartRegistryKey, "StartPos");

            if (_taskShedulerService.CheckIsExist(TaskSchedulerType.Restart))
                await _cronService.DisableCron();
        }

        private Task CreateShortcuts()
        {
            _shortcuts.ForEach(x => _shortcutService.CreateShorcut(x));
            return Task.CompletedTask;
        }

        private readonly List<Shortcut> _shortcuts = new List<Shortcut>
        {
            Shortcut.PcPosDesktop,
            Shortcut.DiagnosticDesktop,
            Shortcut.RepairDesktop,
            Shortcut.SetupDesktop,
            Shortcut.PcPos7,
            Shortcut.Diagnostic,
            Shortcut.Repair,
            Shortcut.Setup,
            Shortcut.Restart,
            Shortcut.Shutdown,
            Shortcut.Disable,
            Shortcut.PcPosTaskband,
            Shortcut.DiagnosticTaskband
        };
    }
}