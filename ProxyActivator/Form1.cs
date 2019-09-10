using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace ProxyActivator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Program was started.";
            notifyIcon.Visible = true;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;

            ShowBalloonTipText(
                "ProxyActivator started.",
                "The Proxy Activator is now running in the background.",
                ToolTipIcon.Info, 600
            );

            ContextMenu menue = new ContextMenu();
            menue.MenuItems.Add(new MenuItem("About..", überToolStripMenuItem1_Click));
            menue.MenuItems.Add(new MenuItem("Exit", beendenToolStripMenuItem_Click));
            notifyIcon.ContextMenu = menue;

            String res = WLanAPManager.Instance.LoadAPsFromFile();
            if (res.Length != 0)
                MessageBox.Show("Couldnt load save file. \n \nError message: " + res, "Error reading save file", MessageBoxButtons.OK, MessageBoxIcon.Error);



            List<WLanAP> wlanaps = WLanAPManager.Instance.GetWLanAPs();
            if (wlanaps.Count != 0)
            {
                dataGridView1.Show();
                dataGridView1.ColumnCount = 4;
                dataGridView1.Columns[0].Name = "AP Name";
                dataGridView1.Columns[1].Name = "Proxy IP";
                dataGridView1.Columns[2].Name = "Proxy Port";
                dataGridView1.Columns[3].Name = "Exceptions";
                foreach (WLanAP ap in WLanAPManager.Instance.GetWLanAPs())
                {
                    dataGridView1.Rows.Add(ap.APName, ap.proxyIP, ap.proxyPort,ap.Exceptions);
                }
            }
            else dataGridView1.Hide();

        }

        private void ShowBalloonTipText(string title, string text, ToolTipIcon icon, int time)
        {
            notifyIcon.BalloonTipText = text;
            notifyIcon.BalloonTipTitle = title;
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.ShowBalloonTip(time);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.Hide();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }


        private void SetText(ref Label label, State state)
        {
            label.ForeColor = state.Color;
            label.Text = state.Text;
        }

        private void ChangeConnectedStatus(Color col, string text)
        {
            L_Connected.ForeColor = col;
            L_Connected.Text = text;
        }

        private void WLanCheck_Tick(object sender, EventArgs e)
        {
            // El proxy debe estar activado
            if (WlanManager.Instance.IsConnectedToAnySSID())
            {
                //
                Boolean alreadyConfigured = false;
                Boolean anyDefinedAPConfigured = false;

                WLanAP connectedNotConfiguredAP = null;

                foreach (WLanAP ap in WLanAPManager.Instance.GetWLanAPs())
                {
                    if (WlanManager.Instance.IsConnectedToSSID(ap.APName))
                    {
                        if (ProxyManager.Instance.ConfiguredProxyName.Equals(ap.APName))
                        {
                            alreadyConfigured = true;
                        }
                        anyDefinedAPConfigured = true;
                        connectedNotConfiguredAP = ap;
                        break;
                    }
                }

                if (connectedNotConfiguredAP != null)
                {
                    if (!alreadyConfigured)
                    {
                        ChangeConnectedStatus(Color.Green, "Connected to " + connectedNotConfiguredAP.APName + ". Proxy enabled. (" + connectedNotConfiguredAP.proxyIP + ":" + connectedNotConfiguredAP.proxyPort + ")");
                        ProxyToggleAll(true, connectedNotConfiguredAP.proxyIP, connectedNotConfiguredAP.proxyPort, connectedNotConfiguredAP.APName, connectedNotConfiguredAP.Exceptions);
                        ProxyManager.Instance.ConfiguredProxyName = connectedNotConfiguredAP.APName;
                        
                        L_Proxy_System.ForeColor = ProxyManager.Instance.ProxyStateSystem.Color;
                        L_Proxy_System.Text = ProxyManager.Instance.ProxyStateSystem.Text;

                    }
                }
                else if (!anyDefinedAPConfigured)
                {
                    ChangeConnectedStatus(Color.Red, "Not connected to any defined wireless network.");
                    ProxyToggleAll(false);
                }

                //
            }
            // El proxy debe estar apagado
            else
            {
                ChangeConnectedStatus(Color.Orange, "Not connected to any wireless network");

                ProxyToggleAll(false);
            }

        }

        private void ProxyToggleAll(bool active, string ip = "", int port = 0, string proxyName = "", string exeptions = "")
        {
            if (active == false)
            {
                // Cerrar
                //if (ProxyManager.Instance.ConfiguredProxyName.Length != 0)
                //{
                    ShowBalloonTipText("Deactivated", "Proxy was deactivated.", ToolTipIcon.Info, 2000);
                    ProxyManager.Instance.ProxyToggleAll(active, ip, port, proxyName, exeptions);
                //}
            }
            else
            {
                ShowBalloonTipText("Activated", "Proxy was activated. \nAP Name: " + proxyName + "\nProxy: " + ip + ":" + port + ".", ToolTipIcon.Info, 2000);
                ProxyManager.Instance.ProxyToggleAll(active, ip, port, proxyName, exeptions);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {


        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void notifyIcon_MouseClick_1(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                this.Visible = false;
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Visible = true;
                this.Focus();
            }
        }

        private void programmMitWindowsStartenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey AutostartKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                AutostartKey.SetValue("FS-ProxyActivator-Start", Application.ExecutablePath.ToString());
                AutostartKey.Close();
                //MessageBox.Show("The program will now start automatically with Windows.", "Successfully", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                programmMitWindowsStartenToolStripMenuItem.Checked=true;
            }
            catch
            {
                MessageBox.Show("Could not describe the registry.", "Error with the entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void programmNichtMitWindowsStartenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                RegistryKey AutostartKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                AutostartKey.DeleteValue("FS-ProxyActivator-Start");
                AutostartKey.Close();
                //MessageBox.Show("The program is no longer started with Windows.", "Erfolgreich", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                programmMitWindowsStartenToolStripMenuItem.Checked = false;
            }
            catch
            {
                MessageBox.Show("Could not describe the registry.\n Was the program started as administrator?", "Error with the entry", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the Proxy Activator?", "Shut down", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                ProxyToggleAll(false);
                Application.Exit();
            }
            else
            {
                return;
            }
        }

        private void überToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("https://github.com/ulises2k/ProxyActivator", "About Proxy Activator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void proxyAktivierenToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void proxyDeaktivierenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProxyManager.Instance.ProxyToggleAll(false);
            //MessageBox.Show("All proxy settings have been deleted", "Successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }
        

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void L_Connected_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
