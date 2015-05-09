using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace klsdamfviu
{
    public partial class connForm : Form
    {
        public connForm()
        {
            InitializeComponent();
        }

        public static string MainNick = "", SecNick = "", ThirdNick = "", Uname = "";

        private void connForm_Load(object sender, EventArgs e)
        {
            this.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + ": Connection Settings";
            loadConfig(AppDomain.CurrentDomain.BaseDirectory + @"\config.xml");
        }

        public void loadConfig(string file)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);
            XmlNodeList nodeList = xmlDoc.DocumentElement.SelectNodes("/config");

            foreach (XmlNode node in nodeList)
            {
                MainNick = node.SelectSingleNode("firstnick").InnerText;
                SecNick = node.SelectSingleNode("secnick").InnerText;
                ThirdNick = node.SelectSingleNode("thirdnick").InnerText;
                Uname = node.SelectSingleNode("uname").InnerText;
            }

            txtFirstNick.Text = MainNick;
            txtSecNick.Text = SecNick;
            txtThirdNick.Text = ThirdNick;
            txtUname.Text = Uname;


        }

    }
}
