using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace klsdamfviu
{
    public class xmlDocs
    {

        public string MainNick { get; set; }
        public string SecNick { get; set; }
        public string ThirdNick { get; set; }
        public string Uname { get; set; }
        public string LastChan { get; set; }
        public string file { get; set; }
        public string Version { get; set; }
        public string Server { get; set; }

        public void loadXMl(string file)
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
                LastChan = node.SelectSingleNode("lastchan").InnerText;
                Version = node.SelectSingleNode("apversion").InnerText;
                Server = node.SelectSingleNode("server").InnerText;
            }
        }
    }
}

