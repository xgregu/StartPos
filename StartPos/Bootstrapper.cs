using Csc.ApiClient;
using NLog;
using StartPos.Forms;
using StartPos.Interfaces;
using StartPos.Models;
using StartPos.Services;
using StartPos.Setup;
using StartPos.Shared;
using StartPos.Shared.Interfaces;
using StartPos.Shared.Services;
using StartPos.Shared.Utils;
using StartPos.Update;
using StartPos.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Unity;
using Unity.Lifetime;

namespace StartPos
{
    internal class Bootstrapper
    {
        private static readonly string _appName = Assembly.GetEntryAssembly()?.GetName().Name;
        private static readonly string _appVersion = Assembly.GetEntryAssembly()?.GetName().Version.ToString();
        private readonly string _arg;

        private readonly IUnityContainer _container;
        private readonly ILogger _logger;

        public Bootstrapper(string argument)
        {
            if (Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location)).Length > 1)
            {
                MessageBox.Show("Powtórne uruchomienie StartPos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            LogManager.Configuration = LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.ApplicationExit += OnApplicationClose;

            _logger = LogManager.GetCurrentClassLogger();
            _logger.Info($"Start application {_appName} v{_appVersion}");
            _container = new UnityContainer();
            _arg = argument.ToUpper();

            RegisterTypes();
            ResolveTypes();
            Upgrade();

            if (string.Equals(_arg, Constants.Flow.Setup) || string.Equals(_arg, Constants.Flow.SetupSilent))
                StartSetup();
            else
                StartApp();
        }

        private void Upgrade()
        {
            var updater = _container.Resolve<IUpdater>();
            updater.CheckForUpdate();
        }

        private void ResolveTypes()
        {
            _container.Resolve<IConfigInitializer>().Initialize(_container);
            var context = _container.Resolve<IContext>();
            context.ActiveFlow = _arg;
            _container.Resolve<RegisterFlow>().RegistrationFlow();
        }

        private void RegisterTypes()
        {
            _logger.Debug("Register types");
            _container.RegisterType<IConfigInitializer, ConfigInitializer>(new TransientLifetimeManager());
            _container.RegisterType<ISqlInstanceService, SqlInstanceService>(new TransientLifetimeManager());
            _container.RegisterType<IRemoteServerService, RemoteServerService>(new TransientLifetimeManager());
            _container.RegisterType<IBackupService, BackupService>(new TransientLifetimeManager());
            _container.RegisterType<IPcPosLogParser, PcPosLogParser>(new TransientLifetimeManager());
            _container.RegisterSingleton<IPcPosInfo, PcPosInfo>();
            _container.RegisterType<IPcPosService, PcPosService>(new TransientLifetimeManager());
            _container.RegisterType<IPcPosConfigParser, PcPosConfigParser>(new TransientLifetimeManager());
            _container.RegisterType<IFirewallService, FirewallService>(new TransientLifetimeManager());
            _container.RegisterType<IMenuStartService, MenuStartService>(new TransientLifetimeManager());
            _container.RegisterType<ICronService, CronService>(new TransientLifetimeManager());
            _container.RegisterSingleton<IContext, Context>();
            _container.RegisterType<IShortcutService, ShortcutService>(new TransientLifetimeManager());
            _container.RegisterType<ISystemService, SystemService>(new TransientLifetimeManager());
            _container.RegisterType<ICscApiClient, CscApiClient>(new TransientLifetimeManager());
            _container.RegisterType<ISqlDatabaseService, SqlDatabaseService>(new TransientLifetimeManager());
            _container.RegisterType<ITaskShedulerService, TaskSchedulerService>(new TransientLifetimeManager());
            _container.RegisterType<IUpdater, Updater>(new TransientLifetimeManager());
            _container.RegisterType<RegisterFlow>(new TransientLifetimeManager());
            _container.RegisterType<IInstallService, InstallService>(new TransientLifetimeManager());
            _container.RegisterSingleton<MainForm>();
            _container.RegisterSingleton<SetupForm>();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal((Exception)e.ExceptionObject);
            Environment.Exit(1);
        }

        private void StartApp() => Program.RunApp(_container);

        private void StartSetup() => Program.RunSetup(_container);

        private void OnApplicationClose(object sender, EventArgs e) => _logger.Info($"Close application {_appName} v{_appVersion}");
    }
}