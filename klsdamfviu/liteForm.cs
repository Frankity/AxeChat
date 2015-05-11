using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace klsdamfviu
{
    public partial class liteForm : Form
    {
        public ScintillaNET.Scintilla scs = new ScintillaNET.Scintilla();

        public liteForm()
        {
            InitializeComponent();
            scs.Enabled = false;
            Font ft = new Font("Consolas", 9.0f);
            scs.Font = ft;
            scs.Dock = DockStyle.Fill;
            this.Controls.Add(scs);
            //scs.Click += scs_click();
        }

    }
}
