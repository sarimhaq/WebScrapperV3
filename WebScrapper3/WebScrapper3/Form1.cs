using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace WebScrapper3
{
    public partial class Form1 : Form
    {

        string MainURL = "http://www.kijiji.ca/b-photography-video/gta-greater-toronto-area/page-6/c168l1700272";
        string HostName = "http://www.kijiji.ca";
        List<string> LeadEmail = new List<string>();
        int subleadcounter = 0;
        string NextURL;



        public Form1()
        {
            InitializeComponent();
        }




        public string GetRequestString(string RequestURL) {
            try {
                string HTMLPage;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RequestURL);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream resStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(resStream))
                {
                    HTMLPage = reader.ReadToEnd();
                }

                return HTMLPage;

            }

            catch(Exception e)
            {
                
                return e.Message;
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++) {    
            string MainPageContent = GetRequestString(MainURL);
            WebBrowser MainBrowser = new WebBrowser();
            MainBrowser.ScriptErrorsSuppressed = true;
            MainBrowser.DocumentText = "0";
            MainBrowser.Document.OpenNew(true);
            MainBrowser.Document.Write(MainPageContent);
            MainBrowser.Refresh();

            var SubLinkLeads = MainBrowser.Document.GetElementsByTagName("a");
            foreach (HtmlElement link in SubLinkLeads)
            {
                    if (link.InnerHtml== "Next&nbsp;&gt;")
                    {
                        var a = HostName;
                        string b = link.GetAttribute("href");
                        var c = b.Substring(6, b.Length - 6);
                        string SubPageUrl = a + c;
                        MainURL = SubPageUrl;


                    }

                if(link.GetAttribute("className")== "title enable-search-navigation-flag" || ("className")== "title" )
                {
                    var a = HostName;
                    string b = link.GetAttribute("href");
                    var c = b.Substring(6, b.Length - 6);
                    string SubPageUrl = a + c;
                    string SubPageContent = GetRequestString(SubPageUrl);
                    WebBrowser SubBrowser = new WebBrowser();
                    SubBrowser.ScriptErrorsSuppressed = true;
                    SubBrowser.DocumentText = "0";
                    SubBrowser.Document.OpenNew(true);
                    SubBrowser.Document.Write(SubPageContent);
                    SubBrowser.Refresh();

                    var LeadContent = SubBrowser.Document.GetElementById("UserContent");
                        Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase); // for emails

                        //Regex reg = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); 

                        MatchCollection Gold = reg.Matches(LeadContent.InnerHtml);

                    if(Gold.Count != 0 )
                    {
                        foreach (Match GoldMatch in Gold)
                        {
                            LeadEmail.Add(GoldMatch.Value);
                        }
                        
                    }



                    



                }
                }

            }


            using (StreamWriter theWriter = new StreamWriter(Application.StartupPath + "\\filekijijiVTestEmails.csv"))
            {

                foreach (string name in LeadEmail)
                {
                    theWriter.WriteLine(name);
                }



            }





        }
    }
}
