using BitDesktop.Properties;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace BitDesktop
{
    public partial class OptionsForm : Form
    {
        private readonly MainWindow _mainWindow;

        public OptionsForm(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            chckAutorun.Checked = Settings.Default.Autorun;
            chckLock.Checked = Settings.Default.Lock;
            txtAmount.Text = Settings.Default.BtcAmount.ToString();
            txtPrice.Text = Settings.Default.BtcPrice.ToString();
            txtFixed.Text = Settings.Default.BtcFixed.ToString();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Settings.Default.Autorun = chckAutorun.Checked;
            Settings.Default.Lock = chckLock.Checked;
            Settings.Default.BtcAmount = float.Parse(txtAmount.Text);
            txtPrice.Text = txtPrice.Text.Split(',')[0];
            Settings.Default.BtcPrice =  int.Parse(txtPrice.Text);
            Settings.Default.BtcFixed = double.Parse(txtFixed.Text);
            Settings.Default.Save();
            setStartup(Settings.Default.Autorun);
            _mainWindow.UpdateBtcValue(null, null);
            Hide();
        }

        private void ValidateNumber(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                btnApply.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyChar == 44 || e.KeyChar == 46)
            {
                e.KeyChar = (char)44;
                if (sender is TextBox box)
                {
                    e.Handled = box.Text.Contains(",");
                    return;
                }
            }

            e.Handled = !(Char.IsNumber(e.KeyChar) || e.KeyChar == 8);
        }

        private void setStartup(bool enabled)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                        ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (enabled)
                rk.SetValue("BitDesktop", Application.ExecutablePath.ToString());
            else
                rk.DeleteValue("BitDesktop", false);
        }

    }
}
