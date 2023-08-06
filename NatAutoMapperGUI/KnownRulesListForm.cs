using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NatAutoMapperGUI
{
    public partial class KnownRulesListForm : Form
    {
        /// <summary>
        /// Lista di regole note.
        /// </summary>
        private string[] KnownRulesList;
        public KnownRulesListForm(string[] KnownRulesList)
        {
            InitializeComponent();
            this.KnownRulesList = KnownRulesList;
        }

        private void KnownRulesListForm_Load(object sender, EventArgs e)
        {
            foreach (string rule in KnownRulesList)
            {
                KnownRulesListBox.Items.Add(rule);
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {

        }

        private void CancelActionButton_Click(object sender, EventArgs e)
        {

        }
    }
}