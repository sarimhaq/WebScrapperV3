﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace PhotographyWebsitesScrapper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WebBrowser GoodRequest = new WebBrowser();
        String GoodRequestData;

        Regex reg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase);
        List<string> EmailAds = new List<string>();
        List<string> EmailList = new List<string>();
        const string FilePath = @"C:\Users\sarim\Documents\Visual Studio 2015\Projects\WebScrapperV2\WebScrapperV2\bin\Debug\filekijijiTestLinks.csv";
        String HomeUrl;
        String ContactPageUrl;

        public string GetRequestString(string RequestURL)
        {
            try
            {
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

            catch (Exception e)
            {
                if (e.Message.Contains("Bad"))
                {
                
                    GoodRequest.ScriptErrorsSuppressed = true;
                    GoodRequest.AllowNavigation = true;
                    GoodRequest.Navigate(RequestURL);
                    while (GoodRequest.ReadyState != WebBrowserReadyState.Complete)
                    {
                        Application.DoEvents();
                    }

                    return GoodRequest.Document.Body.InnerHtml;


                }

                else { 

                return e.Message;
                }
            }


        }

   
        public WebBrowser BrowserFunction (string HTMLContent)
        {

            try {  
            WebBrowser SubBrowser = new WebBrowser();
            SubBrowser.ScriptErrorsSuppressed = true;
            SubBrowser.DocumentText = "0";
            SubBrowser.Document.OpenNew(true);
            SubBrowser.Document.Write(HTMLContent);
            SubBrowser.Refresh();
            return SubBrowser;
            }
            catch (Exception e)
            {
                return null;
            }

        }


        private void button1_Click(object sender, EventArgs e)
        {
           using (StreamReader r = new StreamReader(FilePath))
            {
                string line;
                while((line = r.ReadLine()) != null)
                {
                    EmailAds.Add(line);
                }
            }

            foreach (string link in EmailAds)
            {
                HomeUrl = link;
                string HomePageContent = GetRequestString(link);
                Match Gold = reg.Match(HomePageContent);

                if(Gold.Value != "")
                {
                    EmailList.Add(Gold.Value);

                }
                else
                { 
                WebBrowser HomePageBrowser = new WebBrowser();
                HomePageBrowser = BrowserFunction(HomePageContent);
                var SubLinksLeads = HomePageBrowser.Document.GetElementsByTagName("a");
                foreach(HtmlElement alink in SubLinksLeads)
                    {
                        if (alink.InnerHtml != null && alink.GetAttribute("href") != null) { 
                        if(alink.InnerHtml.Contains("contact") || alink.GetAttribute("href").Contains("contact"))
                        {
                                string SubPageUrl;
                            var a = HomeUrl;
                            string b = alink.GetAttribute("href");
                                if (b.Contains("about")) { 
                             var c = b.Substring(6, b.Length - 6);
                                    if (c.Contains("blank"))
                                    {
                                        c = c.Substring(7, c.Length - 7);
                                    }

                                SubPageUrl = a + c;
                                }
                                else
                                {
                                    SubPageUrl = b;
                                }

                                string AboutPageContent = GetRequestString(SubPageUrl);
                           
                            MatchCollection Gold2 = reg.Matches(HomePageContent);

                                if (Gold2.Count != 0)
                            {
                                foreach(Match GoldMatch in Gold2) { 
                                EmailList.Add(GoldMatch.Value);
                                break;
                                    }
                                }
                            else
                            {
                                EmailList.Add(HomeUrl + "no contacts");
                            }

                        }
                        }
                    }

                }





            }

            using (StreamWriter theWriter = new StreamWriter(Application.StartupPath + "\\WebSiteTestEmails.csv"))
            {

                foreach (string name in EmailList)
                {
                    theWriter.WriteLine(name);
                }



            }



        }
    }
}
