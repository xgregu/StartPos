using NLog;
using System;
using System.Windows.Forms;

namespace StartPos.Forms
{
    public partial class PasswordForm : Form
    {
        private readonly ILogger _logger;

        public PasswordForm()
        {
            InitializeComponent();
            Refresh();
            _logger = LogManager.GetCurrentClassLogger();
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            TopMost = true;
            if (txtPassword.Text == "10coma123")
            {
                _logger.Info($"{nameof(OnOkClick)} | The password is correct");
                DialogResult = DialogResult.OK;
            }
            else
            {
                Hide();
                txtPassword.Clear();
                _logger.Warn($"{nameof(OnOkClick)} | The password is incorrect");
                if (MessageBox.Show("Podane hasło jest nieprawidłowe", "Błędne hasło", MessageBoxButtons.OK,
                    MessageBoxIcon.Stop) != DialogResult.OK)
                {
                    return;
                }
                txtPassword.Focus();
                Show();
            }
        }
    }
}