using System;
using System.Drawing;
using System.Windows.Forms;

namespace StartPos.Forms
{
    public partial class CountdownForm : Form
    {
        private int _countdownTimeLeft;

        public CountdownForm()
        {
            Shown += OnShown;
            InitializeComponent();
            _countdownTimeLeft = 30;
            txtTimer.Text = $"{_countdownTimeLeft}s";
            TopMost = true;
            TopLevel = true;
        }

        private void OnShown(object sender, EventArgs e)
        {
            Focus();
            Refresh();
            timer1.Tick += timer1_Tick;
            timer1.Interval = 1000;
            timer1.Start();
        }

        private void OnButtonStopClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            _countdownTimeLeft -= 1;

            if (_countdownTimeLeft <= 0)
                DialogResult = DialogResult.OK;
            else if (_countdownTimeLeft <= 5)
                txtTimer.ForeColor = Color.Red;
            else if (_countdownTimeLeft <= 15)
                txtTimer.ForeColor = Color.DarkOrange;

            var timespan = TimeSpan.FromSeconds(_countdownTimeLeft);
            txtTimer.Text = $"{timespan.ToString(@"ss")}s";
        }
    }
}