using System;
using System.Security.AccessControl;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Xml;
using IrcLib;


namespace klsdamfviu
{

    public partial class MainForm : Form
    {
        int createdTab = 0;
        public IrcBot irClient;
        public static string nick = connForm.MainNick;
        IdentListener L = new IdentListener(nick);
        xmlDocs xl = new xmlDocs();
        liteForm newchild = new liteForm();

        public MainForm()
        {
            IsMdiContainer = true;
            //L.Listen();
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            xl.loadXMl(AppDomain.CurrentDomain.BaseDirectory + @"\config.xml");
            //  MessageBox.Show(xl.LastChan);

        }
        void MainFormLoad(object sender, EventArgs e)
        {
            Console.WriteLine("working");

            irClient = new IrcBot(xl.MainNick);
            irClient.OnConnectEvent += irc_OnConnectEvent;
            irClient.OnMotdEvent += irc_OnMotEvent;
            irClient.OnChannelMessageEvent += irc_OnChannelMessageEvent;
            irClient.OnChannelMessageEvent +=irClient_OnChannelMessageEvent;
            irClient.OnQueryMessageEvent +=irClient_OnQueryMessageEvent;
            irClient.OnCtcpResponseEvent += irClient_OnCtcpResponseEvent;
            irClient.OnNamereplyEvent += irClient_OnNamereplyEvent;
            irClient.OnPartChannelEvent +=  irClient_OnPartChannelEvent;
            irClient.OnTopicMessageEvent +=irClient_OnTopicMessageEvent;
            irClient.OnNoticeEvent+=irClient_OnNoticeEvent;
            irClient.OnTopicChangedMessageEvent+=irClient_OnTopicChangedMessageEvent;
            irClient.OnNickChangeEvent +=irClient_OnNickChangeEvent;
            irClient.OnJoinChannelEvent += irClient_OnJoinChannelEvent;
            irClient.OnActionEvent += irClient_OnActionEvent;
            
            this.button2.Text = xl.MainNick;

            if (xl.Version != "") { irClient.VersionMessage = xl.Version ; }
            else
            {
                irClient.VersionMessage = "AxeChat v. 11.5.15 [" + System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + "] using RobbingHood Library.";
            }
            List<string> ch = new List<string>();
            ch.Add(xl.LastChan);
          

          irClient.Connect(xl.Server, 6660,true,ch);
        }

        private void irClient_OnActionEvent(string channel, string user, string message)
        {
           newchild.scs.Text += message.ToString();
        }

        private void irClient_OnJoinChannelEvent(string channel, string user)
        {
           //newchild.scs.Text += "Notice: " + user + " has joined on " + channel + "\r\n";
           updatelist();
        }

        private void irClient_OnPartChannelEvent(string channel, string user, string message)
        {
           newchild.scs.Text += "Notice: " + user + " has quit" + "\r\n";
            updatelist();
        }

        public void AddNewChan(string chann)
        {
            TabPage childtab = new TabPage();
            newchild.MdiParent = this;
            newchild.Name = "page" + createdTab.ToString();
            newchild.Text = chann;
            childtab.Name = newchild.Name;
            childtab.Text = newchild.Text;
            tabControl1.TabPages.Add(childtab);
            newchild.scs.Parent = childtab;
            ToolStripMenuItem menutab = new ToolStripMenuItem();
            menutab.Text = newchild.Text;
            menutab.Name = newchild.Name;
            herramientasToolStripMenuItem.DropDownItems.Add(menutab);
            menutab.Click += new EventHandler(menutab_Click);
            tabControl1.SelectTab(childtab);
            newchild.Show();
            createdTab++;
        }

        private void menutab_Click(object sender, EventArgs e)
        {
         //   throw new NotImplementedException();
            foreach (TabPage theTab in tabControl1.TabPages)
            {
                if (theTab.Text == sender.ToString())
                {
                    tabControl1.SelectTab(theTab);
                    foreach (Form WantToSelect in this.MdiChildren)
                    {
                        if (WantToSelect.Name == theTab.Name)
                        {
                            WantToSelect.Select();
                        }
                    }
                }
            }

        }

        public void updatelist()
        {
            irClient.SendRaw("NAMES " + tabControl1.SelectedTab.Text);
        }

        private void irClient_OnNickChangeEvent(string oldNick, string newNick)
        {
            newchild.scs.Text += "Notice: " + oldNick + " has changed his nick to " + newNick + "\r\n";
        }

        private void irClient_OnTopicChangedMessageEvent(string changedBy, string channel, string topic)
        {
       /*     Font f = new Font("Consolas",9.0f,FontStyle.Bold);
            int str = newrtb.Find(changedBy);
            if (str > 0)
	            {
                    newrtb.Select(str,changedBy.Length);
                    newrtb.SelectionFont = new System.Drawing.Font(newrtb.Font, FontStyle.Bold);
                }
            newrtb.Text += changedBy + " has changed the topic to: " + topic + "\r\n";*/
        }

        private void irClient_OnNoticeEvent(IrcMessage m)
        {
            infortb.Text += m.Message.ToString() + "\r\n";
        }

        private void irClient_OnTopicMessageEvent(string channel, string topic)
        {
           //newchild.scs.Text += "The topic for: " + channel + " is " + topic + "\n";
            textBox2.Text += topic; 
        }

        private void irClient_OnNamereplyEvent(string channel, string[] users)
        {
            listView1.Clear();
            for (int i = 0; i < users.Length; i++)
            {
             
                listView1.Items.Add(users[i]);
                if (users[i].StartsWith("&"))
                {
                    listView1.Items[0].ImageIndex = 0;
                }
                else if (users[i].StartsWith("@"))
                {
                    listView1.Items[0].ImageIndex = 0;
                }
            }

            toolStripStatusLabel2.Text = users.Length.ToString();
        }

        private void irClient_OnNamereplyEvent(IrcMessage m)
        {
             newchild.scs.Text += m.Message.ToString() + "\r\n";
        }
        
        void irClient_OnCtcpResponseEvent(string sender, string command)
        {
           newchild.scs.Text += sender + " " + command;
        }

        private void irClient_OnQueryMessageEvent(IrcMessage m)
        {
            infortb.Text += m.Message.ToString() + "\r\n";
        }

        private void irClient_OnChannelMessageEvent(IrcMessage m)
        {
            if (m.Message.StartsWith(":")) 
            {
                m.Message.Replace(":",":");
            }else{
               infortb.Text += m.Message.ToString() + "\r\n";
            }
        }
        
        private void irc_OnChannelMessageEvent(IrcMessage m)
        {
            if (newchild.scs.InvokeRequired)
            {
                newchild.scs.Text += m.SenderName.ToString() + ": " + m.Message.ToString() + Environment.NewLine;
            }
        }

        private void irc_OnConnectEvent()
        {
            infortb.Text += "Connected\n";
            AddNewChan(xl.LastChan.ToString());
        }

        private void irc_OnMotEvent(IrcMessage m)
        {
            if (infortb.InvokeRequired)
            {
                infortb.Text += m.Message.ToString() + "\r\n";
            }
        }

       /* public void maketab(string name)
        {
            TabPage newchan = new TabPage(name);
            newchan.Name = name;
            tabControl1.SelectedTab = newchan;
            tabControl1.TabPages.Add(newchan);

            newrtb.Enabled = false;
            Font ft = new Font("Consolas", 9.0f);
            newrtb.Font = ft;
            newrtb.Dock = DockStyle.Fill;
            
            newchan.Controls.Add(newrtb);
            newrtb.TextChanged +=newrtb_TextChanged;
        }*/

        /*private void newrtb_TextChanged(object sender, EventArgs e)
        {
         //   ScintillaNET.Scintilla bold = newrtb;
         
        }*/

        private void irc_OnJoinChannelEvent(string chann)
        {
            //MessageBox.Show("hpppppppppppppppppppppppppppppppp");
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Dispose();
            irClient.LeaveChannel(xl.LastChan);
            irClient.Disconnect();
            System.Environment.Exit(1);
        }

        void chanjoin(string channame)
        {
            irClient.JoinChannel(channame);
            AddNewChan(channame);
        }

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chanjoin(textBox3.Text);   
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.tabControl1.SelectedTab.Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            irClient.JoinChannel(textBox3.Text);

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                    irClient.Send(tabControl1.SelectedTab.Text, textBox1.Text);
                    newchild.scs.Text += this.button2.Text + ": " + textBox1.Text + "\r\n";
                    textBox1.Clear();
                    textBox1.Focus();
            }
        }

        private void opcionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connForm cf = new connForm();
            cf.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            nickForm nf = new nickForm();
            nf.ShowDialog();

            try
            {
                irClient.SendRaw("NICK " + nf.textBox1.Text);
                this.button2.Text = nf.textBox1.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

       

        private void tabControl1_MouseClick(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(tabControl1.SelectedTab.Name);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            foreach (liteForm WantToSelect in this.MdiChildren)
            {
                if (tabControl1.SelectedTab != null)
                {
                    if (WantToSelect.Name == tabControl1.SelectedTab.Name)
                    {
                        WantToSelect.Select();
                        
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            updatelist();
        }

      

    }
}
