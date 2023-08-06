namespace NatAutoMapperGUI
{
    partial class NatAutoMapperGUIForm
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NatAutoMapperGUIForm));
            this.ClosePortSplitContainer = new System.Windows.Forms.SplitContainer();
            this.CurrentRulesListBox = new System.Windows.Forms.ListBox();
            this.RemainingLifetimeValueLabel = new System.Windows.Forms.Label();
            this.RemainingLifetimeLabel = new System.Windows.Forms.Label();
            this.ExternalPortValueLabel = new System.Windows.Forms.Label();
            this.ClosePortExternalPort = new System.Windows.Forms.Label();
            this.InternalPortValueLabel = new System.Windows.Forms.Label();
            this.ClosePortInternalPort = new System.Windows.Forms.Label();
            this.IpAddressValueLabel = new System.Windows.Forms.Label();
            this.ClosePortIpAddressLabel = new System.Windows.Forms.Label();
            this.ProtocolTypeLabel = new System.Windows.Forms.Label();
            this.ClosePortProtocolLabel = new System.Windows.Forms.Label();
            this.SinglePortOpenGroupBox = new System.Windows.Forms.GroupBox();
            this.RuleDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.RuleDescriptionLabel = new System.Windows.Forms.Label();
            this.RuleLifetimeLabel = new System.Windows.Forms.Label();
            this.RuleLifetimeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.PublicPortLabel = new System.Windows.Forms.Label();
            this.PublicPortTextBox = new System.Windows.Forms.TextBox();
            this.PrivatePortLabel = new System.Windows.Forms.Label();
            this.PrivatePortTextBox = new System.Windows.Forms.TextBox();
            this.UseDeviceIPCheckBox = new System.Windows.Forms.CheckBox();
            this.IpAddressLabel = new System.Windows.Forms.Label();
            this.IpAddressTextBox = new System.Windows.Forms.TextBox();
            this.ProtocolComboBox = new System.Windows.Forms.ComboBox();
            this.ProtocolLabel = new System.Windows.Forms.Label();
            this.ClosePortGroupBox = new System.Windows.Forms.GroupBox();
            this.OpenPortButton = new System.Windows.Forms.Button();
            this.ClosePortButton = new System.Windows.Forms.Button();
            this.OtherOperationsLabel = new System.Windows.Forms.Label();
            this.AppMainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.ScriptMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GenerateScriptSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RunScriptSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenPortsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenKnownPortSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AppSettingsSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolSettingsSubMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePortSplitContainer)).BeginInit();
            this.ClosePortSplitContainer.Panel1.SuspendLayout();
            this.ClosePortSplitContainer.Panel2.SuspendLayout();
            this.ClosePortSplitContainer.SuspendLayout();
            this.SinglePortOpenGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RuleLifetimeNumericUpDown)).BeginInit();
            this.ClosePortGroupBox.SuspendLayout();
            this.AppMainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ClosePortSplitContainer
            // 
            resources.ApplyResources(this.ClosePortSplitContainer, "ClosePortSplitContainer");
            this.ClosePortSplitContainer.Name = "ClosePortSplitContainer";
            // 
            // ClosePortSplitContainer.Panel1
            // 
            resources.ApplyResources(this.ClosePortSplitContainer.Panel1, "ClosePortSplitContainer.Panel1");
            this.ClosePortSplitContainer.Panel1.Controls.Add(this.CurrentRulesListBox);
            // 
            // ClosePortSplitContainer.Panel2
            // 
            resources.ApplyResources(this.ClosePortSplitContainer.Panel2, "ClosePortSplitContainer.Panel2");
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.RemainingLifetimeValueLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.RemainingLifetimeLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.ExternalPortValueLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.ClosePortExternalPort);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.InternalPortValueLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.ClosePortInternalPort);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.IpAddressValueLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.ClosePortIpAddressLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.ProtocolTypeLabel);
            this.ClosePortSplitContainer.Panel2.Controls.Add(this.ClosePortProtocolLabel);
            // 
            // CurrentRulesListBox
            // 
            resources.ApplyResources(this.CurrentRulesListBox, "CurrentRulesListBox");
            this.CurrentRulesListBox.FormattingEnabled = true;
            this.CurrentRulesListBox.Name = "CurrentRulesListBox";
            this.CurrentRulesListBox.SelectedIndexChanged += new System.EventHandler(this.CurrentRulesListBox_SelectedIndexChanged);
            // 
            // RemainingLifetimeValueLabel
            // 
            resources.ApplyResources(this.RemainingLifetimeValueLabel, "RemainingLifetimeValueLabel");
            this.RemainingLifetimeValueLabel.Name = "RemainingLifetimeValueLabel";
            // 
            // RemainingLifetimeLabel
            // 
            resources.ApplyResources(this.RemainingLifetimeLabel, "RemainingLifetimeLabel");
            this.RemainingLifetimeLabel.Name = "RemainingLifetimeLabel";
            // 
            // ExternalPortValueLabel
            // 
            resources.ApplyResources(this.ExternalPortValueLabel, "ExternalPortValueLabel");
            this.ExternalPortValueLabel.Name = "ExternalPortValueLabel";
            // 
            // ClosePortExternalPort
            // 
            resources.ApplyResources(this.ClosePortExternalPort, "ClosePortExternalPort");
            this.ClosePortExternalPort.Name = "ClosePortExternalPort";
            // 
            // InternalPortValueLabel
            // 
            resources.ApplyResources(this.InternalPortValueLabel, "InternalPortValueLabel");
            this.InternalPortValueLabel.Name = "InternalPortValueLabel";
            // 
            // ClosePortInternalPort
            // 
            resources.ApplyResources(this.ClosePortInternalPort, "ClosePortInternalPort");
            this.ClosePortInternalPort.Name = "ClosePortInternalPort";
            // 
            // IpAddressValueLabel
            // 
            resources.ApplyResources(this.IpAddressValueLabel, "IpAddressValueLabel");
            this.IpAddressValueLabel.Name = "IpAddressValueLabel";
            // 
            // ClosePortIpAddressLabel
            // 
            resources.ApplyResources(this.ClosePortIpAddressLabel, "ClosePortIpAddressLabel");
            this.ClosePortIpAddressLabel.Name = "ClosePortIpAddressLabel";
            // 
            // ProtocolTypeLabel
            // 
            resources.ApplyResources(this.ProtocolTypeLabel, "ProtocolTypeLabel");
            this.ProtocolTypeLabel.Name = "ProtocolTypeLabel";
            // 
            // ClosePortProtocolLabel
            // 
            resources.ApplyResources(this.ClosePortProtocolLabel, "ClosePortProtocolLabel");
            this.ClosePortProtocolLabel.Name = "ClosePortProtocolLabel";
            // 
            // SinglePortOpenGroupBox
            // 
            resources.ApplyResources(this.SinglePortOpenGroupBox, "SinglePortOpenGroupBox");
            this.SinglePortOpenGroupBox.Controls.Add(this.RuleDescriptionTextBox);
            this.SinglePortOpenGroupBox.Controls.Add(this.RuleDescriptionLabel);
            this.SinglePortOpenGroupBox.Controls.Add(this.RuleLifetimeLabel);
            this.SinglePortOpenGroupBox.Controls.Add(this.RuleLifetimeNumericUpDown);
            this.SinglePortOpenGroupBox.Controls.Add(this.PublicPortLabel);
            this.SinglePortOpenGroupBox.Controls.Add(this.PublicPortTextBox);
            this.SinglePortOpenGroupBox.Controls.Add(this.PrivatePortLabel);
            this.SinglePortOpenGroupBox.Controls.Add(this.PrivatePortTextBox);
            this.SinglePortOpenGroupBox.Controls.Add(this.UseDeviceIPCheckBox);
            this.SinglePortOpenGroupBox.Controls.Add(this.IpAddressLabel);
            this.SinglePortOpenGroupBox.Controls.Add(this.IpAddressTextBox);
            this.SinglePortOpenGroupBox.Controls.Add(this.ProtocolComboBox);
            this.SinglePortOpenGroupBox.Controls.Add(this.ProtocolLabel);
            this.SinglePortOpenGroupBox.Name = "SinglePortOpenGroupBox";
            this.SinglePortOpenGroupBox.TabStop = false;
            // 
            // RuleDescriptionTextBox
            // 
            resources.ApplyResources(this.RuleDescriptionTextBox, "RuleDescriptionTextBox");
            this.RuleDescriptionTextBox.Name = "RuleDescriptionTextBox";
            // 
            // RuleDescriptionLabel
            // 
            resources.ApplyResources(this.RuleDescriptionLabel, "RuleDescriptionLabel");
            this.RuleDescriptionLabel.Name = "RuleDescriptionLabel";
            // 
            // RuleLifetimeLabel
            // 
            resources.ApplyResources(this.RuleLifetimeLabel, "RuleLifetimeLabel");
            this.RuleLifetimeLabel.Name = "RuleLifetimeLabel";
            // 
            // RuleLifetimeNumericUpDown
            // 
            resources.ApplyResources(this.RuleLifetimeNumericUpDown, "RuleLifetimeNumericUpDown");
            this.RuleLifetimeNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.RuleLifetimeNumericUpDown.Name = "RuleLifetimeNumericUpDown";
            // 
            // PublicPortLabel
            // 
            resources.ApplyResources(this.PublicPortLabel, "PublicPortLabel");
            this.PublicPortLabel.Name = "PublicPortLabel";
            // 
            // PublicPortTextBox
            // 
            resources.ApplyResources(this.PublicPortTextBox, "PublicPortTextBox");
            this.PublicPortTextBox.Name = "PublicPortTextBox";
            // 
            // PrivatePortLabel
            // 
            resources.ApplyResources(this.PrivatePortLabel, "PrivatePortLabel");
            this.PrivatePortLabel.Name = "PrivatePortLabel";
            // 
            // PrivatePortTextBox
            // 
            resources.ApplyResources(this.PrivatePortTextBox, "PrivatePortTextBox");
            this.PrivatePortTextBox.Name = "PrivatePortTextBox";
            // 
            // UseDeviceIPCheckBox
            // 
            resources.ApplyResources(this.UseDeviceIPCheckBox, "UseDeviceIPCheckBox");
            this.UseDeviceIPCheckBox.Name = "UseDeviceIPCheckBox";
            this.UseDeviceIPCheckBox.UseVisualStyleBackColor = true;
            this.UseDeviceIPCheckBox.CheckedChanged += new System.EventHandler(this.UseDeviceIPCheckBox_CheckedChanged);
            // 
            // IpAddressLabel
            // 
            resources.ApplyResources(this.IpAddressLabel, "IpAddressLabel");
            this.IpAddressLabel.Name = "IpAddressLabel";
            // 
            // IpAddressTextBox
            // 
            resources.ApplyResources(this.IpAddressTextBox, "IpAddressTextBox");
            this.IpAddressTextBox.Name = "IpAddressTextBox";
            this.IpAddressTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.IpAddressTextBox_Validating);
            // 
            // ProtocolComboBox
            // 
            resources.ApplyResources(this.ProtocolComboBox, "ProtocolComboBox");
            this.ProtocolComboBox.FormattingEnabled = true;
            this.ProtocolComboBox.Items.AddRange(new object[] {
            resources.GetString("ProtocolComboBox.Items"),
            resources.GetString("ProtocolComboBox.Items1"),
            resources.GetString("ProtocolComboBox.Items2")});
            this.ProtocolComboBox.Name = "ProtocolComboBox";
            // 
            // ProtocolLabel
            // 
            resources.ApplyResources(this.ProtocolLabel, "ProtocolLabel");
            this.ProtocolLabel.Name = "ProtocolLabel";
            // 
            // ClosePortGroupBox
            // 
            resources.ApplyResources(this.ClosePortGroupBox, "ClosePortGroupBox");
            this.ClosePortGroupBox.Controls.Add(this.ClosePortSplitContainer);
            this.ClosePortGroupBox.Name = "ClosePortGroupBox";
            this.ClosePortGroupBox.TabStop = false;
            // 
            // OpenPortButton
            // 
            resources.ApplyResources(this.OpenPortButton, "OpenPortButton");
            this.OpenPortButton.Name = "OpenPortButton";
            this.OpenPortButton.UseVisualStyleBackColor = true;
            this.OpenPortButton.Click += new System.EventHandler(this.OpenPortButton_Click);
            // 
            // ClosePortButton
            // 
            resources.ApplyResources(this.ClosePortButton, "ClosePortButton");
            this.ClosePortButton.Name = "ClosePortButton";
            this.ClosePortButton.UseVisualStyleBackColor = true;
            this.ClosePortButton.Click += new System.EventHandler(this.ClosePortButton_Click);
            // 
            // OtherOperationsLabel
            // 
            resources.ApplyResources(this.OtherOperationsLabel, "OtherOperationsLabel");
            this.OtherOperationsLabel.Name = "OtherOperationsLabel";
            // 
            // AppMainMenuStrip
            // 
            resources.ApplyResources(this.AppMainMenuStrip, "AppMainMenuStrip");
            this.AppMainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ScriptMenuItem,
            this.OpenPortsMenuItem,
            this.SettingsMenuItem});
            this.AppMainMenuStrip.Name = "AppMainMenuStrip";
            // 
            // ScriptMenuItem
            // 
            resources.ApplyResources(this.ScriptMenuItem, "ScriptMenuItem");
            this.ScriptMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GenerateScriptSubMenuItem,
            this.RunScriptSubMenuItem});
            this.ScriptMenuItem.Name = "ScriptMenuItem";
            // 
            // GenerateScriptSubMenuItem
            // 
            resources.ApplyResources(this.GenerateScriptSubMenuItem, "GenerateScriptSubMenuItem");
            this.GenerateScriptSubMenuItem.Name = "GenerateScriptSubMenuItem";
            this.GenerateScriptSubMenuItem.Click += new System.EventHandler(this.GenerateScriptSubMenuItem_Click);
            // 
            // RunScriptSubMenuItem
            // 
            resources.ApplyResources(this.RunScriptSubMenuItem, "RunScriptSubMenuItem");
            this.RunScriptSubMenuItem.Name = "RunScriptSubMenuItem";
            this.RunScriptSubMenuItem.Click += new System.EventHandler(this.RunScriptSubMenuItem_Click);
            // 
            // OpenPortsMenuItem
            // 
            resources.ApplyResources(this.OpenPortsMenuItem, "OpenPortsMenuItem");
            this.OpenPortsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenKnownPortSubMenuItem});
            this.OpenPortsMenuItem.Name = "OpenPortsMenuItem";
            // 
            // OpenKnownPortSubMenuItem
            // 
            resources.ApplyResources(this.OpenKnownPortSubMenuItem, "OpenKnownPortSubMenuItem");
            this.OpenKnownPortSubMenuItem.Name = "OpenKnownPortSubMenuItem";
            this.OpenKnownPortSubMenuItem.Click += new System.EventHandler(this.OpenKnownPortSubMenuItem_Click);
            // 
            // SettingsMenuItem
            // 
            resources.ApplyResources(this.SettingsMenuItem, "SettingsMenuItem");
            this.SettingsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AppSettingsSubMenuItem,
            this.ToolSettingsSubMenuItem});
            this.SettingsMenuItem.Name = "SettingsMenuItem";
            // 
            // AppSettingsSubMenuItem
            // 
            resources.ApplyResources(this.AppSettingsSubMenuItem, "AppSettingsSubMenuItem");
            this.AppSettingsSubMenuItem.Name = "AppSettingsSubMenuItem";
            this.AppSettingsSubMenuItem.Click += new System.EventHandler(this.AppSettingsSubMenuItem_Click);
            // 
            // ToolSettingsSubMenuItem
            // 
            resources.ApplyResources(this.ToolSettingsSubMenuItem, "ToolSettingsSubMenuItem");
            this.ToolSettingsSubMenuItem.Name = "ToolSettingsSubMenuItem";
            this.ToolSettingsSubMenuItem.Click += new System.EventHandler(this.ToolSettingsSubMenuItem_Click);
            // 
            // NatAutoMapperGUIForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.OtherOperationsLabel);
            this.Controls.Add(this.ClosePortButton);
            this.Controls.Add(this.OpenPortButton);
            this.Controls.Add(this.ClosePortGroupBox);
            this.Controls.Add(this.SinglePortOpenGroupBox);
            this.Controls.Add(this.AppMainMenuStrip);
            this.MaximizeBox = false;
            this.Name = "NatAutoMapperGUIForm";
            this.ShowIcon = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NatAutoMapperGUIForm_FormClosing);
            this.Load += new System.EventHandler(this.NatAutoMapperGUIForm_Load);
            this.Shown += new System.EventHandler(this.NatAutoMapperGUIForm_Shown);
            this.ClosePortSplitContainer.Panel1.ResumeLayout(false);
            this.ClosePortSplitContainer.Panel2.ResumeLayout(false);
            this.ClosePortSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ClosePortSplitContainer)).EndInit();
            this.ClosePortSplitContainer.ResumeLayout(false);
            this.SinglePortOpenGroupBox.ResumeLayout(false);
            this.SinglePortOpenGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RuleLifetimeNumericUpDown)).EndInit();
            this.ClosePortGroupBox.ResumeLayout(false);
            this.AppMainMenuStrip.ResumeLayout(false);
            this.AppMainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox SinglePortOpenGroupBox;
        private System.Windows.Forms.ComboBox ProtocolComboBox;
        private System.Windows.Forms.Label ProtocolLabel;
        private System.Windows.Forms.CheckBox UseDeviceIPCheckBox;
        private System.Windows.Forms.Label IpAddressLabel;
        private System.Windows.Forms.TextBox IpAddressTextBox;
        private System.Windows.Forms.TextBox RuleDescriptionTextBox;
        private System.Windows.Forms.Label RuleDescriptionLabel;
        private System.Windows.Forms.Label RuleLifetimeLabel;
        private System.Windows.Forms.NumericUpDown RuleLifetimeNumericUpDown;
        private System.Windows.Forms.Label PublicPortLabel;
        private System.Windows.Forms.TextBox PublicPortTextBox;
        private System.Windows.Forms.Label PrivatePortLabel;
        private System.Windows.Forms.TextBox PrivatePortTextBox;
        private System.Windows.Forms.GroupBox ClosePortGroupBox;
        private System.Windows.Forms.SplitContainer ClosePortSplitContainer;
        private System.Windows.Forms.ListBox CurrentRulesListBox;
        private System.Windows.Forms.Label RemainingLifetimeValueLabel;
        private System.Windows.Forms.Label RemainingLifetimeLabel;
        private System.Windows.Forms.Label ExternalPortValueLabel;
        private System.Windows.Forms.Label ClosePortExternalPort;
        private System.Windows.Forms.Label InternalPortValueLabel;
        private System.Windows.Forms.Label ClosePortInternalPort;
        private System.Windows.Forms.Label IpAddressValueLabel;
        private System.Windows.Forms.Label ClosePortIpAddressLabel;
        private System.Windows.Forms.Label ProtocolTypeLabel;
        private System.Windows.Forms.Label ClosePortProtocolLabel;
        private System.Windows.Forms.Button OpenPortButton;
        private System.Windows.Forms.Button ClosePortButton;
        private System.Windows.Forms.Label OtherOperationsLabel;
        private System.Windows.Forms.MenuStrip AppMainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ScriptMenuItem;
        private System.Windows.Forms.ToolStripMenuItem GenerateScriptSubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RunScriptSubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenPortsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenKnownPortSubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AppSettingsSubMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolSettingsSubMenuItem;
    }
}

