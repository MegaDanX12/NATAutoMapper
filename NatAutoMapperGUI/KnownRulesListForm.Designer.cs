namespace NatAutoMapperGUI
{
    partial class KnownRulesListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KnownRulesListForm));
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelActionButton = new System.Windows.Forms.Button();
            this.KnownRulesListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            resources.ApplyResources(this.OKButton, "OKButton");
            this.OKButton.Name = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelActionButton
            // 
            resources.ApplyResources(this.CancelActionButton, "CancelActionButton");
            this.CancelActionButton.Name = "CancelActionButton";
            this.CancelActionButton.UseVisualStyleBackColor = true;
            this.CancelActionButton.Click += new System.EventHandler(this.CancelActionButton_Click);
            // 
            // KnownRulesListBox
            // 
            this.KnownRulesListBox.FormattingEnabled = true;
            resources.ApplyResources(this.KnownRulesListBox, "KnownRulesListBox");
            this.KnownRulesListBox.Name = "KnownRulesListBox";
            // 
            // KnownRulesListForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.KnownRulesListBox);
            this.Controls.Add(this.CancelActionButton);
            this.Controls.Add(this.OKButton);
            this.Name = "KnownRulesListForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.KnownRulesListForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CancelActionButton;
        private System.Windows.Forms.ListBox KnownRulesListBox;
    }
}