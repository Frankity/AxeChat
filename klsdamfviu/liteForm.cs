using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ICSharpCode.TextEditor;


namespace klsdamfviu
{
    public partial class liteForm : Form
    {
        public ICSharpCode.TextEditor.TextEditorControl scs = new TextEditorControl();

        public liteForm()
        {
            InitializeComponent();
            scs.IsReadOnly = true;
            Font ft = new Font("Consolas", 9.0f);
            scs.Font = ft;
            scs.Dock = DockStyle.Fill;
            scs.HideMouseCursor = true;
            this.Controls.Add(scs);
            
        
        }


    }
}
