using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace D3_Bot_Tool
{
    public partial class UpdaterInfo : Form
    {
        public UpdaterInfo(String name)
        {
            InitializeComponent();
            this.Text = "Updating " + name + " ...";
        }

        private void UpdaterInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void UpdaterInfo_Load(object sender, EventArgs e)
        {

        }
    }
}
