using Csc.ApiClient;
using NLog;
using StartPos.Interfaces;
using StartPos.Shared;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

// ReSharper disable All

namespace StartPos.Forms
{
    internal partial class MainForm : Form
    {
        private const int _cP_NOCLOSE_BUTTON = 0x200;
        private readonly IContext _context;
        private readonly IFlow _flow;
        private readonly ILogger _windowLogger;
        private readonly ICscApiClient _apiClient;

        public MainForm(IContext context, IFlow flow, ICscApiClient apiClient)
        {
            _context = context;
            _flow = flow;

            InitializeComponent();
            Load += OnLoad;
            Shown += OnShown;
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _apiClient = apiClient;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var myCp = base.CreateParams;
                myCp.ClassStyle |= _cP_NOCLOSE_BUTTON;
                return myCp;
            }
        }

        private async void OnShown(object sender, EventArgs e)
        {
            Text = $"StartPos v.{_context.AppVersion}";
            if (!string.IsNullOrEmpty(_context.ActiveFlow)) Text += $"  -   {_context.ActiveFlow.ToUpper()}";
            _windowLogger.Info($"Start programu StartPos - v {_context.AppVersion}");
            Refresh();

            _apiClient.TrySendOldReports();

            if (!_context.PcPosInfo.IsInstalled && _flow.Name != Constants.Flow.BuildInstaller)
            {
                _windowLogger.Error("Nie wykryto zainstalowanego oprogramowania Pc-Pos");
                _context.TsReport.Fatal += $" ► Nie wykryto zainstalowanego oprogramowania Pc-Pos{Environment.NewLine}";
            }
            else
                await _flow.Run();

            if (String.IsNullOrEmpty(_context.ActiveFlow))
                _apiClient.SendReport(_context.TsReport);

            await Task.Delay(2500);

            while (Opacity >= 0.01)
            {
                Opacity -= 0.05;
                await Task.Delay(50);
            }

            Close();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            LogManager.Configuration = LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();
        }
    }
}