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

       // public RichTextBox newrtb = new RichTextBox();
        public ScintillaNET.Scintilla newrtb = new ScintillaNET.Scintilla();

        public MainForm()
        {
            IsMdiContainer = true;
            //L.Listen();
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            xl.loadXMl(AppDomain.CurrentDomain.BaseDirectory + @"\config.xml");
            MessageBox.Show(xl.LastChan);
        }

        void MainFormLoad(object sender, EventArgs e)
        {

            irClient = new IrcBot(xl.MainNick);
            irClient.OnConnectEvent += irc_OnConnectEvent;
            irClient.OnMotdEvent += irc_OnMotEvent;
            
            
            //irClient.OnJoinChannelEvent +=irClient_OnJoinChannelEvent;
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
            ch.Add("#dahouse");
          

          irClient.Connect(xl.Server, 6660,true,ch);
        }

        private void irClient_OnActionEvent(string channel, string user, string message)
        {
            newrtb.Text += message.ToString();
        }

        private void irClient_OnJoinChannelEvent(string channel, string user)
        {
           newrtb.Text += "Notice: " + user + " has joined on " + channel + "\r\n";
           updatelist();
        }

        private void irClient_OnPartChannelEvent(string channel, string user, string message)
        {
          //  irClient.Send(xl.LastChan, irClient.QuitMessage.ToString());
            //newrtb.Text += xl.MainNick + ": " + irClient.QuitMessage.ToString() + "\r\n";
            newrtb.Text += "Notice: " + user + " has quit" + "\r\n";
            updatelist();
        }

        public void AddNewChan(string chann)
        {
            
            liteForm newchild = new liteForm();
            TabPage childtab = new TabPage();
            newchild.MdiParent = this;
            newchild.Name = "child " + createdTab.ToString();
            newchild.Text = " child no " + createdTab.ToString();
            childtab.Name = newchild.Name;
            childtab.Text = newchild.Text;
            tabControl1.TabPages.Add(childtab);
            newchild.scs.Parent = childtab;

            newchild.Show();
            createdTab++;
        }

        public void updatelist()
        {
            irClient.SendRaw("NAMES " + xl.LastChan);
        }

        private void irClient_OnNickChangeEvent(string oldNick, string newNick)
        {
            newrtb.Text += "Notice: " + oldNick + " has changed his nick to " + newNick + "\r\n";
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
            newrtb.Text += "The topic for: " + channel + " is " + topic + "\n";
            textBox2.Text += topic; 
        }

        private void irClient_OnNamereplyEvent(string channel, string[] users)
        {
            listView1.Clear();
            for (int i = 0; i < users.Length; i++)
            {
                IList<ScintillaNET.Range> r = newrtb.FindReplace.FindAll(i.ToString());
                //newrtb.FindReplace.FindAll(i.ToString());
                newrtb.FindReplace.HighlightAll(r);
                listView1.Items.Add(users[i]);
            }
        }

        private void irClient_OnNamereplyEvent(IrcMessage m)
        {
            newrtb.Text += m.Message.ToString() + "\r\n";
        }
        
        void irClient_OnCtcpResponseEvent(string sender, string command)
        {
            newrtb.Text += sender + " " + command;
        }

        private void irClient_OnQueryMessageEvent(IrcMessage m)
        {
            infortb.Text += m.Message.ToString() + "\r\n";
        }

        private void irClient_OnChannelMessageEvent(IrcMessage m)
        {
            infortb.Text += m.Message.ToString() + "\r\n";
        }
        
        private void irc_OnChannelMessageEvent(IrcMessage m)
        {
            if (newrtb.InvokeRequired)
            {
                newrtb.Text += m.SenderName.ToString() + ": " + m.Message.ToString() + Environment.NewLine;
            }
        }

        private void irc_OnConnectEvent()
        {
            infortb.Text += "Connected\n";
            maketab(xl.LastChan.ToString());
        }

        private void irc_OnMotEvent(IrcMessage m)
        {
            if (infortb.InvokeRequired)
            {
                infortb.Text += m.Message.ToString() + "\r\n";
            }
        }

        public void maketab(string name)
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
        }

        private void newrtb_TextChanged(object sender, EventArgs e)
        {
         //   ScintillaNET.Scintilla bold = newrtb;
         
        }

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

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewChan(xl.LastChan);
            //maketab(xl.LastChan);
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.tabControl1.SelectedTab.Name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Focus();
            irClient.SendCtcpRequest(button2.Text, "NAMES ");
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (textBox1.Text.StartsWith("//"))
                {
                    irClient.SendRaw("/" + textBox1.Text);
                    newrtb.Text += this.button2.Text + ": " + textBox1.Text + "\r\n";
                    textBox1.Clear();
                    textBox1.Focus();
                }
                else
                {
                    irClient.Send(xl.LastChan, textBox1.Text);
                    newrtb.Text += this.button2.Text + ": " + textBox1.Text + "\r\n";
                    textBox1.Clear();
                    textBox1.Focus();
                }
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

    }
}
