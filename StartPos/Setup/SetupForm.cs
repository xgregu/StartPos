using Csc.ApiClient;
using Microsoft.WindowsAPICodePack.Dialogs;
using NLog;
using StartPos.Forms;
using StartPos.Interfaces;
using StartPos.Services;
using StartPos.Shared;
using StartPos.Shared.Configs;
using StartPos.Shared.Interfaces;
using StartPos.Shared.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StartPos.Setup
{
    internal partial class SetupForm : Form
    {
        private readonly ICscApiClient _apiClient;
        private readonly IConfig _config;
        private readonly IContext _context;
        private readonly SetupFlow _flow;
        private readonly IRemoteServerService _remoteServerService;
        private readonly ISqlDatabaseService _sqlDatabaseService;
        private readonly ISqlInstanceService _sqlInstanceService;
        private readonly IBackupService _backupService;
        private readonly ILogger _windowLogger;
        private string _instanceName;
        private bool _isPasswordChecked;
        private readonly IPcPosService _pcPosService;
        private Dictionary<string, string> _oldConfig;
        private readonly string _javaPath;

        public SetupForm(SetupFlow flow, IContext context, IBackupService backupService, IConfig config, ISqlInstanceService sqlInstanceService, ICscApiClient cscApiClient, ISqlDatabaseService sqlDatabaseService, IRemoteServerService remoteServerService, IPcPosService pcPosService)
        {
            InitializeComponent();

            Cursor.Current = Cursors.WaitCursor;

            Load += OnLoad;
            Shown += OnShown;
            _flow = flow;
            _context = context;
            _config = config;
            _sqlInstanceService = sqlInstanceService;
            _apiClient = cscApiClient;
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _sqlDatabaseService = sqlDatabaseService;
            _remoteServerService = remoteServerService;
            _backupService = backupService;
            Cursor.Current = Cursors.Default;
            _pcPosService = pcPosService;
            _javaPath = Path.Combine(_context.PcPosInfo.InstalationDir, "Java", "jre", "bin", "java.exe");
        }

        private void PrepareMain()
        {
            ReadInstanceInSystem();

            if (_context.PcPosInfo.SerializationInfo != null)
            {
                foreach (var item in _context.PcPosInfo.SerializationInfo)
                {
                    infoSerial.AppendText(item + Environment.NewLine);
                    infoSerial.Select(0, 0);
                    infoSerial.ScrollToCaret();
                }
            }
            if (File.Exists(_javaPath))
            {
                var javaInfo = FileVersionInfo.GetVersionInfo(_javaPath);
                informacje_wersjaJava.Text = javaInfo.ProductName + " v." + javaInfo.FileVersion;
            }

            daneKlienta_nazwaFirmy.Text = _config.Contractor.Name;
            daneKlienta_lokalizacja_textbox.Text = _config.Contractor.LocationName;

            sql_baza.Text = _context.PcPosInfo.DataBase;
            sql_serwer.Text = _config.Database.Host;
            sql_instancja.Text = _instanceName = _config.Database.Instance;
            sql_port.Value = _context.PcPosInfo.Port;
            sql_haslo.Text = _config.Database.Password;

            backup_glowny_Sciezka.Text = _config.ObligatoryBackup.Path;
            backup_dodatkowy_Sciezka.Text = _config.AlternativeBackup.Path;
            IsAlternativeBackup.Checked = _config.AlternativeBackup.Active;
            inne_sklepCalodobowy.Checked = _config.IsAllDayShop;

            inne_monitorKlienta.Checked = _config.IsClientMonitorInstalled;
            inne_stanowisko.Text = _context.PcPosInfo.PosNumber;
            inne_serwerPcMarket.Text = $"{_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort}";

            automatyczneAktualizacje_insoft.Checked = _config.IsInsoftUpdateServerInstalled;

            Task.Run(CheckSqlConnection);
            Task.Run(CheckContractor);
            Task.Run(CheckWriteAccessObligatoryBackup);
            Task.Run(CheckWriteAccessAlternativeBackup);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            LogManager.Configuration = LogManager.Configuration.Reload();
            LogManager.ReconfigExistingLoggers();
        }

        private void OnSaveButtonClick(object sender, EventArgs e) => SaveConfig();

        private void OnSqlShowPasswordClick(object sender, EventArgs e)
        {
            if (sql_pokazHaslo.Checked && !_isPasswordChecked)
            {
                sql_pokazHaslo.Checked = false;
                sql_haslo.PasswordChar = '*';
            }
            else
            {
                sql_pokazHaslo.Checked = true;
                _isPasswordChecked = false;
                sql_haslo.PasswordChar = '\0';
            }
        }

        private void UpdateConfig()
        {
            var textToAdd = $"-{_config.AppToRun.ToUpper()}-ST-{inne_stanowisko.Text}";
            if (!daneKlienta_lokalizacja_textbox.Text.Contains(textToAdd))
                daneKlienta_lokalizacja_textbox.Text = $"{daneKlienta_lokalizacja_textbox.Text}{textToAdd}";

            _windowLogger.Info($"Aktualizacja pliku {Constants.ConfigFile}");

            _config.Contractor.IsContract = usText.ForeColor == Color.Green;
            _config.Contractor.Name = daneKlienta_nazwaFirmy.Text;
            _config.Contractor.LocationName = daneKlienta_lokalizacja_textbox.Text;
            _config.Database.Host = sql_serwer.Text;
            _config.Database.Instance = sql_instancja.Text;
            _config.Database.Password = sql_haslo.Text;
            _config.ObligatoryBackup.Active = true;
            _config.ObligatoryBackup.Path = string.IsNullOrEmpty(backup_glowny_Sciezka.Text)
                ? @"C:\Pcmwin\StartPos_Backup"
                : backup_glowny_Sciezka.Text;

            _config.AlternativeBackup.Active = IsAlternativeBackup.Checked;

            if (IsAlternativeBackup.Checked)
            {
                _config.AlternativeBackup.Active = !string.IsNullOrEmpty(backup_dodatkowy_Sciezka.Text);
                if (_config.AlternativeBackup.Active)
                    _config.AlternativeBackup.Path = backup_dodatkowy_Sciezka.Text;
            }
            else
            {
                _config.AlternativeBackup.Path = string.Empty;
            }

            _config.IsAllDayShop = inne_sklepCalodobowy.Checked;
            _config.IsClientMonitorInstalled = inne_monitorKlienta.Checked;
            _config.IsInsoftUpdateServerInstalled = automatyczneAktualizacje_insoft.Checked;
            _config.IsStartPosEnabled = true;

            _config.SaveSettings();
            XmlHelpers.DeserializeXml<Config>(Constants.ConfigFile);
        }

        private void OnSqlShowPasswordCheckedChanged(object sender, EventArgs e) => _isPasswordChecked = sql_pokazHaslo.Checked;

        private void OnSqlInstanceDropDown(object sender, EventArgs e) => sql_instancja.ForeColor = Color.Black;

        private void OnSqlInstanceNameTextLeave(object sender, EventArgs e)
        {
            _instanceName = sql_instancja.Text;
            sql_instancja.ForeColor = _sqlInstanceService.IsInstanceExist(_instanceName) ? Color.Green : Color.Red;
            Task.Run(CheckSqlConnection);
        }

        private void CheckSqlConnection()
        {
            sql_sprawdzPolaczenieWynik.ForeColor = Color.DarkOrange;
            sql_sprawdzPolaczenieWynik.ForeColor =
                _sqlDatabaseService.IsConnectionCorrectly(sql_serwer.Text, _instanceName, sql_port.Value, sql_baza.Text,
                    sql_haslo.Text)
                    ? Color.Green
                    : Color.Red;
        }

        private async Task CheckContractor()
        {
            if (String.IsNullOrEmpty(daneKlienta_nazwaFirmy.Text))
                return;

            daneKlienta_nazwaFirmy.ForeColor = Color.DarkOrange;
            usText.ForeColor = Color.DarkOrange;

            if (string.IsNullOrEmpty(daneKlienta_nazwaFirmy.Text))
                return;

            var client = await _apiClient.GetCustomer(daneKlienta_nazwaFirmy.Text);

            if (!client.IsSuccess)
            {
                daneKlienta_nazwaFirmy.ForeColor = Color.Black;
                usText.ForeColor = Color.Black;
                return;
            }

            if (!client.IsValid)
            {
                daneKlienta_nazwaFirmy.ForeColor = Color.Red;
                usText.ForeColor = Color.Black;
                return;
            }

            daneKlienta_nazwaFirmy.ForeColor = Color.Green;

            usText.ForeColor = client.Customer.IsServiceContract ? Color.Green : Color.Red;
        }

        private void ReadInstanceInSystem() => _sqlInstanceService.GetAllInstance().ForEach(x => sql_instancja.Items.Add(x));

        private void OnContractorNameLeave(object sender, EventArgs e) => Task.Run(CheckContractor);

        private void OnContractorNameTextChanged(object sender, EventArgs e) => daneKlienta_nazwaFirmy.ForeColor = usText.ForeColor = Color.Black;

        private void OnObligatoryBackupPathClick(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.IsFolderPicker = true;
                    dialog.Title = "StartPos - wskaż lokalizacje backup";
                    dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        backup_glowny_Sciezka.Text = dialog.FileName;
                }
            }
            catch
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                    dialog.Title = "StartPos - wskaż lokalizacje backup";
                    if (dialog.ShowDialog() == DialogResult.OK)
                        backup_dodatkowy_Sciezka.Text = dialog.FileName;
                }
            }
            Task.Run(CheckWriteAccessObligatoryBackup);
        }

        private void OnAlternativeBackupPathClick(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new CommonOpenFileDialog())
                {
                    dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                    dialog.Title = "StartPos - wskaż lokalizacje backup";
                    dialog.IsFolderPicker = true;
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                        backup_dodatkowy_Sciezka.Text = dialog.FileName;
                }
            }
            catch
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.InitialDirectory = Path.GetPathRoot(Environment.SystemDirectory);
                    dialog.Title = "StartPos - wskaż lokalizacje backup";
                    if (dialog.ShowDialog() == DialogResult.OK)
                        backup_dodatkowy_Sciezka.Text = dialog.FileName;
                }
            }
            Task.Run(CheckWriteAccessAlternativeBackup);
        }

        private async void OnShown(object sender, EventArgs e)
        {
            Text = $"StartPos - Konfigurator v.{_context.AppVersion}";
            daneKlienta_groupBox.Visible = sql_GroupBox.Visible = sql_GroupBox.Visible = groupBox2.Visible = groupBox1.Visible = backup_groupBox.Visible = convertButton.Visible = saveButton.Visible = label4.Visible = label9.Visible = false;

            using (var passwordForm = new PasswordForm())
            {
                if (passwordForm.ShowDialog() != DialogResult.OK)
                    Close();
            }

            if (!_context.PcPosInfo.IsInstalled)
            {
                MessageBox.Show("Nie wykryto zainstalowanego oprogramowania Pc-Pos. \n\nDo poprawnej pracy StartPos, niezbędna jest instalacja oraz konfiguracja oprogramowania Pc-Pos", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            PrepareMain();

            if (_context.ActiveFlow == Constants.Flow.SetupSilent)
            {
                var oldConfigFile = Path.Combine("C:\\", "Pcmwin", "StartPos", "konfig", "konfig.cfg");
                if (!File.Exists(oldConfigFile))
                    return;

                ReadingOldConfig(oldConfigFile);
                ParseOldConfig();
                await SaveConfig();
            }

            daneKlienta_groupBox.Visible = sql_GroupBox.Visible = sql_GroupBox.Visible = groupBox2.Visible = groupBox1.Visible = backup_groupBox.Visible = convertButton.Visible = saveButton.Visible = label4.Visible = label9.Visible = true;
            Refresh();
        }

        private void OnSqlHostTextChanged(object sender, EventArgs e) => Task.Run(CheckSqlConnection);

        private void OnSqlPortValueChanged(object sender, EventArgs e) => Task.Run(CheckSqlConnection);

        private void OnSqlPasswordTextChanged(object sender, EventArgs e) => Task.Run(CheckSqlConnection);

        private void OnSaveButtonMouseLeave(object sender, EventArgs e) => saveButton.BorderStyle = BorderStyle.None;

        private void OnSaveButtonMouseHover(object sender, EventArgs e) => saveButton.BorderStyle = BorderStyle.Fixed3D;

        private void OnConvertButtonMouseLeave(object sender, EventArgs e) => convertButton.BorderStyle = BorderStyle.None;

        private void OnConvertButtonMouseHover(object sender, EventArgs e) => convertButton.BorderStyle = BorderStyle.Fixed3D;

        private void OnServerPcMarketHostTextChanged(object sender, EventArgs e)
        {
            inne_serwerPcMarket.Text =
                string.Format($"{_context.PcPosInfo.RemoteServerIP}:{_context.PcPosInfo.RemoteServerPort}");
            Task.Run(CheckServerPcMarketConnection);
        }

        private void CheckServerPcMarketConnection()
        {
            inne_serwerPcMarket.ForeColor = Color.DarkOrange;
            inne_serwerPcMarket.ForeColor =
                _remoteServerService.IsActive(_context.PcPosInfo.RemoteServerIP, _context.PcPosInfo.RemoteServerPort)
                    ? Color.Green
                    : Color.Red;
        }

        private void CheckWriteAccessObligatoryBackup()
        {
            backup_glowny_Sciezka.ForeColor = Color.DarkOrange;
            backup_glowny_Sciezka.ForeColor = _backupService.IsWriteAccessToFolder(backup_glowny_Sciezka.Text) ? Color.Green : Color.Red;
        }

        private void CheckWriteAccessAlternativeBackup()
        {
            backup_dodatkowy_Sciezka.ForeColor = Color.DarkOrange;
            backup_dodatkowy_Sciezka.ForeColor = _backupService.IsWriteAccessToFolder(backup_dodatkowy_Sciezka.Text) ? Color.Green : Color.Red;
        }

        private void OnIsAlternativeBackupCheckedChanged(object sender, EventArgs e) => backup_dodatkowy_Sciezka.Enabled = IsAlternativeBackup.Checked;

        private void SetupForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
                SaveConfig();
        }

        private async Task SaveConfig()
        {
            KeyPreview = false;
            Enabled = false;

            UpdateConfig();

            MainTab.SelectedTab = Log;
            MainTab.TabPages.Remove(Setup);

            if (_config.IsInsoftUpdateServerInstalled)
            {
                var isUpdateInsoftUpdateFiles = MessageBox.Show(
                    "Czy aktualizować pliki dotyczące serwera aktualizacji insoft? \n\n" +
                    "Dotyczy plików: \n" +
                    $"{_context.PcPosInfo.InstalationDir}\\pcpos.conf.\n" +
                    "     Sekcja: SoftwareUpdate\n" +
                    $"{_context.PcPosInfo.InstalationDir}\\launcher.conf",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (isUpdateInsoftUpdateFiles == DialogResult.Yes)
                    _pcPosService.UpdateInsoftUpdateFiles();
            }

            if (_config.IsClientMonitorInstalled)
            {
                var isUpdateInsoftUpdateFiles = MessageBox.Show(
                    "Czy aktualizować pliki dotyczące konfiguracji monitora klienta? \n\n" +
                    "Dotyczy plików: \n" +
                    $"{_context.PcPosInfo.InstalationDir}\\pcpos.conf.\n" +
                    "     Sekcja: CashierEvents\n",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (isUpdateInsoftUpdateFiles == DialogResult.Yes)
                    _pcPosService.ClientMonitorUpdateFiles();
            }

            Refresh();
            await _flow.Run();

            _windowLogger.Info("Koniec pracy");
            await Task.Delay(2500);

            while (Opacity >= 0.01)
            {
                Opacity -= 0.05;
                await Task.Delay(50);
            }
            Close();
        }

        private void OndaneKlienta_lokalizacja_textbox_Leave(object sender, EventArgs e)
        {
            var textToAdd = $"-{_config.AppToRun.ToUpper()}-ST-{inne_stanowisko.Text}";
            if (!daneKlienta_lokalizacja_textbox.Text.Contains(textToAdd))
                daneKlienta_lokalizacja_textbox.Text = $"{daneKlienta_lokalizacja_textbox.Text}{textToAdd}";
        }

        private void OnConvertButtonClick(object sender, EventArgs e)
        {
            string oldConfigPath;

            using (var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Constants.BaseDir,
                Title = "Wskaż plik konfig.cfg",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "cfg",
                Filter = "StartPos console config file (*.cfg)|*.cfg",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            })
                if (fileDialog.ShowDialog() == DialogResult.OK)
                    oldConfigPath = fileDialog.FileName;
                else
                    return;

            ReadingOldConfig(oldConfigPath);
            ParseOldConfig();

            var textToAdd = $"-{_config.AppToRun.ToUpper()}-ST-{inne_stanowisko.Text}";
            if (!daneKlienta_lokalizacja_textbox.Text.Contains(textToAdd))
                daneKlienta_lokalizacja_textbox.Text = $"{daneKlienta_lokalizacja_textbox.Text}{textToAdd}";
            Task.Run(CheckSqlConnection);
            Task.Run(CheckContractor);
            Task.Run(CheckWriteAccessObligatoryBackup);
            Task.Run(CheckWriteAccessAlternativeBackup);
        }

        private void ParseOldConfig()
        {
            daneKlienta_nazwaFirmy.Text = GetOldConfigEntry("Firma");
            daneKlienta_lokalizacja_textbox.Text = GetOldConfigEntry("LokalizacjaPC");
            sql_instancja.Text = GetOldConfigEntry("InstancjaSQL");
            if (int.TryParse(GetOldConfigEntry("PortSQL"), out var port))
                sql_port.Value = port;
            sql_haslo.Text = GetOldConfigEntry("HasloSQL");
            backup_glowny_Sciezka.Text = GetOldConfigEntry("LokalizacjaObowiazkowegoBackupu");
            backup_dodatkowy_Sciezka.Text = GetOldConfigEntry("LokalizacjaAlternatywnegoBackupu");
            IsAlternativeBackup.Checked = ConvertToBool(GetOldConfigEntry("BackupAlternatywny"));
            inne_sklepCalodobowy.Checked = !ConvertToBool(GetOldConfigEntry("AutoRestart"));
            inne_monitorKlienta.Checked = ConvertToBool(GetOldConfigEntry("MonitorKlienta"));
            automatyczneAktualizacje_insoft.Checked = ConvertToBool(GetOldConfigEntry("SerwerAktualizacji"));
        }

        private static bool ConvertToBool(string str) => string.Equals(str, "TAK", StringComparison.InvariantCultureIgnoreCase);

        private void ReadingOldConfig(string confPath)
        {
            var config = new Dictionary<string, string>();

            using (var sr = File.OpenText(confPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("#") || !line.Contains("="))
                        continue;
                    var parm = line.Trim().Split('=');
                    if (parm[0] != "" && parm[1] != "")
                    {
                        config.Add(parm[0].Trim(), parm[1].Trim());
                    }
                }
            }
            _oldConfig = config;
        }

        private string GetOldConfigEntry(string key) => _oldConfig.ContainsKey(key)
            ? _oldConfig[key]
            : string.Empty;

        private void OnSqlInstanceNameTextChanged(object sender, EventArgs e)
        {
            _instanceName = sql_instancja.Text;
            sql_instancja.ForeColor = _sqlInstanceService.IsInstanceExist(_instanceName) ? Color.Green : Color.Red;
        }

        private void OnObligatoryBackupLeave(object sender, EventArgs e) => Task.Run(CheckWriteAccessObligatoryBackup);

        private void OnAlternativeBackupLeave(object sender, EventArgs e) => Task.Run(CheckWriteAccessAlternativeBackup);

        private void OnObligatoryBackupTextChanged(object sender, EventArgs e) => backup_glowny_Sciezka.ForeColor = Color.Black;

        private void OnAlternativeBackupTextChanged(object sender, EventArgs e) => backup_dodatkowy_Sciezka.ForeColor = Color.Black;
    }
}