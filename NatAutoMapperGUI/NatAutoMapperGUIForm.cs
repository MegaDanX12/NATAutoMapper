using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using System.Globalization;

namespace NatAutoMapperGUI
{
    public partial class NatAutoMapperGUIForm : Form
    {
        /// <summary>
        /// Processo dell'utilità a linea di comando.
        /// </summary>
        private Process ToolProcess;
        /// <summary>
        /// Indica che i programmi sono sincronizzati.
        /// </summary>
        private bool Synced = false;
        /// <summary>
        /// Regole attualmente impostate.
        /// </summary>
        private RuleInfo[] Rules;
        public NatAutoMapperGUIForm()
        {
            InitializeComponent();
        }

        private void OpenPortButton_Click(object sender, EventArgs e)
        {
            if (PrivatePortTextBox.Text.Contains('-') && PublicPortTextBox.Text.Contains('-'))
            {
                MessageBox.Show(Properties.Resources.NotImplementedMessage, Properties.Resources.NotImplementedTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //ToolCommunication.OpenPortRange(RuleDescriptionTextBox.Text, (string)ProtocolComboBox.SelectedValue, IpAddressTextBox.Text, PrivatePortTextBox.Text, PublicPortTextBox.Text, Convert.ToInt32(RuleLifetimeNumericUpDown.Value));
            }
            else if ((PrivatePortTextBox.Text.Contains('-') && !PublicPortTextBox.Text.Contains('-')) || (PublicPortTextBox.Text.Contains('-') && !PrivatePortTextBox.Text.Contains('-')))
            {
                MessageBox.Show(Properties.Resources.NotImplementedMessage, Properties.Resources.NotImplementedTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //MessageBox.Show(Properties.Resources.PortRangeInvalidValuesErrorText, Properties.Resources.PortRangeInvalidValuesErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                ToolCommunication.OpenPort(RuleDescriptionTextBox.Text, (string)ProtocolComboBox.SelectedValue, IpAddressTextBox.Text, Convert.ToInt32(PrivatePortTextBox.Text, CultureInfo.InvariantCulture), Convert.ToInt32(PublicPortTextBox.Text, CultureInfo.InvariantCulture), Convert.ToInt32(RuleLifetimeNumericUpDown.Value));
            }
        }

        private void ClosePortButton_Click(object sender, EventArgs e)
        {
            ToolCommunication.ClosePort((string)CurrentRulesListBox.SelectedItem);
        }

        private void NatAutoMapperGUIForm_Load(object sender, EventArgs e)
        {
            LoadSettings();
            ToolCommunication.Initialize();
            string[] Handles = ToolCommunication.GetPipeHandles();
            ProcessStartInfo StartInfo = new ProcessStartInfo("NATAutoMapper.exe", "-gui /input " + Handles[1] + " /output " + Handles[0])
            {
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = false
            };
            ToolProcess = Process.Start(StartInfo);
            ToolProcess.EnableRaisingEvents = true;
            ToolProcess.Exited += ToolProcess_Exited;
            if (!ToolCommunication.SyncProcesses())
            {
                MessageBox.Show(Properties.Resources.NotSyncedErrorMessage, Properties.Resources.NotSyncedErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (Settings.RetrySyncing)
                {
                    for (int i = 0; i < Settings.SyncingRetries; i++)
                    {
                        if (ToolCommunication.SyncProcesses())
                        {
                            Synced = true;
                            break;
                        }
                    }
                    if (!Synced)
                    {
                        MessageBox.Show(Properties.Resources.NotSyncedErrorMessage, Properties.Resources.NotSyncedErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                Synced = true;
            }
        }

        private void ToolProcess_Exited(object sender, EventArgs e)
        {
            if (MessageBox.Show(Properties.Resources.ToolTerminatedErrorMessage, Properties.Resources.ToolTerminatedErrorTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes)
            {
                for (int i = 0; i < Settings.RestartRetries; i++)
                {
                    ToolProcess.Dispose();
                    ToolCommunication.Reinitialize();
                    string[] Handles = ToolCommunication.GetPipeHandles();
                    ProcessStartInfo StartInfo = new ProcessStartInfo("NATAutoMapper.exe", "-gui /input " + Handles[1] + " /output " + Handles[0])
                    {
                        WindowStyle = ProcessWindowStyle.Normal,
                        UseShellExecute = false
                    };
                    ToolProcess = Process.Start(StartInfo);
                    ToolProcess.EnableRaisingEvents = true;
                    ToolProcess.Exited += ToolProcess_Exited;
                    if (!ToolCommunication.SyncProcesses())
                    {
                        MessageBox.Show(Properties.Resources.NotSyncedErrorMessage, Properties.Resources.NotSyncedErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (Settings.RetrySyncing)
                        {
                            for (int j = 0; j < Settings.SyncingRetries; j++)
                            {
                                if (ToolCommunication.SyncProcesses())
                                {
                                    Synced = true;
                                    break;
                                }
                            }
                            if (!Synced)
                            {
                                MessageBox.Show(Properties.Resources.NotSyncedErrorMessage, Properties.Resources.NotSyncedErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        Synced = true;
                    }
                }
            }
        }

        private void UseDeviceIPCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (UseDeviceIPCheckBox.Checked)
            {
                string LocalIP = ToolCommunication.GetLocalIP();
                if (string.IsNullOrWhiteSpace(LocalIP))
                {
                    MessageBox.Show(Properties.Resources.LocalIPUnavailableMessage, Properties.Resources.LocalIPUnavailableTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UseDeviceIPCheckBox.Checked = false;
                    return;
                }
                else
                {
                    IpAddressTextBox.Text = LocalIP;
                    IpAddressTextBox.Enabled = false;
                }
            }
            else
            {
                IpAddressTextBox.Text = string.Empty;
                IpAddressTextBox.Enabled = true;
            }
        }

        private void IpAddressTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(IpAddressTextBox.Text))
            {
                if (!ToolCommunication.IsLocalIP(IpAddressTextBox.Text))
                {
                    MessageBox.Show(Properties.Resources.NotLocalIPErrorMessage, Properties.Resources.NotLocalIPErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    IpAddressTextBox.Text = string.Empty;
                }
            }
        }

        private void NatAutoMapperGUIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ToolProcess.HasExited)
            {
                ToolCommunication.TerminateTool();
            }
            ToolProcess.Dispose();
        }

        private void NatAutoMapperGUIForm_Shown(object sender, EventArgs e)
        {
            Rules = ToolCommunication.RetrieveCurrentRulesList();
            foreach (RuleInfo rule in Rules)
            {
                CurrentRulesListBox.Items.Add(rule.Name);
            }
        }

        private void CurrentRulesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            RuleInfo Info = Rules.First(rule => rule.Name == (string)CurrentRulesListBox.Items[CurrentRulesListBox.SelectedIndex]);
            ProtocolTypeLabel.Text = Info.Protocol;
            IpAddressValueLabel.Text = Info.Address.ToString();
            InternalPortValueLabel.Text = Info.PrivatePort.ToString(CultureInfo.InvariantCulture);
            ExternalPortValueLabel.Text = Info.PublicPort.ToString(CultureInfo.InvariantCulture);
            if (Info.RemainingLifetime == 0)
            {
                RemainingLifetimeValueLabel.Text = Properties.Resources.UndefinedLifetime;
            }
            else
            {
                RemainingLifetimeValueLabel.Text = Info.RemainingLifetime.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void GenerateScriptSubMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void RunScriptSubMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ScriptSelectionOpenFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".nat",
                Filter = Properties.Resources.RunScriptFilterText,
                Multiselect = false,
                SupportMultiDottedExtensions = false,
                Title = Properties.Resources.RunScriptFileDialogTitle
            })
            {
                if (ScriptSelectionOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ToolCommunication.RunScript(ScriptSelectionOpenFileDialog.FileName);
                }
            }
        }

        private void OpenKnownPortSubMenuItem_Click(object sender, EventArgs e)
        {
            string[] RulesList = ToolCommunication.RetrieveKnownPortsList();
            using (KnownRulesListForm form = new KnownRulesListForm(RulesList))
            {
                form.ShowDialog();
            }
        }

        private void AppSettingsSubMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ToolSettingsSubMenuItem_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// Crea il file di impostazioni di default.
        /// </summary>
        private static void CreateDefaultSettings()
        {
            XDocument doc = new XDocument(
                new XElement("Settings",
                    new XElement("BasicSettings",
                        new XElement("RetrySyncing", false.ToString(CultureInfo.InvariantCulture)),
                        new XElement("SyncingRetries", 0.ToString(CultureInfo.InvariantCulture)),
                        new XElement("SyncingWaitTime", 10.ToString(CultureInfo.InvariantCulture)),
                        new XElement("RestartRetries", 1.ToString(CultureInfo.InvariantCulture))
                    )
                )
            );
            doc.Save("Settings.xml");
        }
        /// <summary>
        /// Carica le impostazioni.
        /// </summary>
        private void LoadSettings()
        {
            if (!File.Exists("Settings.xml"))
            {
                CreateDefaultSettings();
            }
            XDocument doc = XDocument.Load("Settings.xml");
            XElement Element = doc.Descendants("RetrySyncing").First();
            Settings.RetrySyncing = bool.Parse(Element.Value);
            Element = doc.Descendants("SyncingRetries").First();
            Settings.SyncingRetries = byte.Parse(Element.Value, CultureInfo.InvariantCulture);
            Element = doc.Descendants("SyncingWaitTime").First();
            Settings.SyncingWaitTime = TimeSpan.FromSeconds(double.Parse(Element.Value, CultureInfo.InvariantCulture));
            Element = doc.Descendants("RestartRetries").First();
            Settings.RestartRetries = byte.Parse(Element.Value, CultureInfo.InvariantCulture);
        }
    }
}