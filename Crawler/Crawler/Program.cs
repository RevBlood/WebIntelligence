using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Collections.Generic;

namespace Crawler {
    class Program {
        static void Main(string[] args) {
            string initialPage = "http://www.oprah.com";

            
            Console.ReadLine();
            //CrawlPages(initialPage);
            indexer index = new indexer();

        }

        static void CrawlPages(string initialPage) {

            string webpage = "";
            string currentMainPage = "";
            string tempPage = "";
            string url;
            string textToMatch = "href=\"([^\"]*)";
            Match match;
            List<string> visitedPages = new List<string>();
            Queue pageQueue = new Queue();

            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\pagefolder\";

            int counter = 0;

            visitedPages.Add(initialPage);
            pageQueue.Enqueue(initialPage);

            Regex regex = new Regex(textToMatch, RegexOptions.IgnoreCase);

            while (pageQueue.Count != 0) {
                if (counter == 5) {
                    break;
                }
                while (webpage == "") {
                    currentMainPage = (string)pageQueue.Peek();
                    webpage = FetchWebPage((string)pageQueue.Dequeue());
                }

                match = regex.Match(webpage);

                while (match.Success) {
                    url = match.Value;
                    if (url.EndsWith(".css") == true) {
                        match = match.NextMatch();
                    } else {
                        url = NormalizeUrl(url, tempPage, currentMainPage);

                        // if (url.EndsWith(".dk") || url.Contains(".dk/")){

                        if (!visitedPages.Contains(url)) {
                            visitedPages.Add(url);
                            pageQueue.Enqueue(url);
                            Console.WriteLine(url);
                        }
                        //}
                        match = match.NextMatch();
                    }
                }

                webpage = removeHead(webpage);
                webpage = removeScriptAndStyle(webpage);
                webpage = saveWebpage(webpage);

                // Write the stream contents to a new file
                using (StreamWriter outfile = new StreamWriter(mydocpath + RemoveSpecialCharacters(currentMainPage) + ".txt")) {
                    outfile.WriteLine(currentMainPage);
                    outfile.Write(webpage.ToString());
                    outfile.Close();
                }

                webpage = "";

                System.Threading.Thread.Sleep(1500);

                counter++;
            }
            return;
        }

        static string NormalizeUrl(string url, string tempPage, string currentMainPage) {

            string webpageToMatch = "http://[a-zA-Z0-9.-]*";
            Regex currentUrlRegex = new Regex(webpageToMatch, RegexOptions.IgnoreCase);
            Match mainPageMatch;

            url = url.Remove(0, 6).TrimEnd('/');


            mainPageMatch = currentUrlRegex.Match(url);
            tempPage = mainPageMatch.Value;

            if (url.StartsWith("//")) {
                url = url.Remove(0, 2);
            }
            if (url.StartsWith("/") == true) {
                url = currentMainPage + url;

            }
            if (url.StartsWith("http://www.") == false) {
                if (url.StartsWith("http://") == true) {
                    url.Insert(8, "www.");
                } else if (url.StartsWith("www.") == true) {
                    url = "http://" + url;
                } else if (url.StartsWith("www.") == false) {
                    url = "http://www." + url;
                }
            }

            // Less important?
            url = url.Substring(0, tempPage.Length).ToLower() + url.Substring(tempPage.Length);

            for (int i = 0; i < url.Length - 3; i++) {
                if (url[i] == '%') {
                    url = url.Substring(0, i + 1) + url.Substring(i + 1, 2).ToUpper() + url.Substring(i + 3, url.Length - 3 - i);
                }
            }

            return url;
        }


        static string saveWebpage(string webpage) {
            const string htmlCode = "<.*?>";
            webpage = Regex.Replace(webpage, htmlCode, string.Empty);
            return webpage;
        }

        static string removeScriptAndStyle(string webpage) {
            var regex = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return regex.Replace(webpage, "");
        }

        static string removeHead(string webpage) {
            Regex rRemScript = new Regex(@"<head [^>]*>[\s\S]*?</head>");
            return rRemScript.Replace(webpage, "");
        }
        public static string RemoveSpecialCharacters(string str) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str) {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_') {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        static string FetchWebPage(string url) {
            string txt = "";

            Console.WriteLine("LOADING URL: " + url);

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse response = null;
                response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK) {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader;
                    reader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                    txt = reader.ReadToEnd();
                    response.Close();
                    reader.Close();
                }

            } catch (Exception e) {
                txt = "";
            }

            return txt;
        }

        static bool RobotViolalationCheck(string url) {

            List<string> disallows = new List<string>();
            bool violation = false;
            int i = url.IndexOf(".dk/") + 4;
            string robotsUrl = url.Substring(0, i) + "robots.txt";
            disallows = RobotParser(robotsUrl, 1);
            foreach (string rule in disallows) {
                if (rule == url) {
                    violation = true;
                }
            }

            return violation;
        }

        static List<string> RobotParser(string url, int ruleType) {
            string userAgentName = "Treeline";
            List<string> disallows = new List<string>();
            List<string> allows = new List<string>();
            List<string> organizedTxt = new List<string>();

            organizedTxt = FetchRobotsTxt(url);
            disallows = FindRobotRules(organizedTxt, userAgentName, 1);
            allows = FindRobotRules(organizedTxt, userAgentName, 2);
            /*Console.WriteLine("Disallows:");
            PrintRobotRules(disallows);
            Console.WriteLine("Allows:");
            PrintRobotRules(allows); */

            return disallows;
        }

        static void PrintRobotRules(List<string> rules) {
            foreach (string rule in rules) {
                Console.WriteLine(rule);
            }
            Console.ReadLine();
        }

        static List<string> FindRobotRules(List<String> organizedTxt, string userAgentName, int ruleType) {
            List<string> rules = new List<string>();

            for (int i = 0; i < organizedTxt.Count - 1; i++) {
                if (organizedTxt[i] == userAgentName) {
                    rules = saveRobotRules(organizedTxt, i, ruleType);
                    break;
                } else if (organizedTxt[i] == "*") {
                    rules = saveRobotRules(organizedTxt, i, ruleType);
                } else { }
            }
            return rules;
        }

        static List<string> saveRobotRules(List<string> organizedTxt, int i, int ruleType) {
            List<string> rules = new List<string>();
            int j = 1;

            string ruleName = GetRobotRuleName(ruleType);

            while (j != 0 && i + j < organizedTxt.Count - 1) {
                if (organizedTxt[i + j] == "User-agent:") {
                    j = 0;
                } else if (organizedTxt[i + j] == ruleName) {
                    rules.Add(organizedTxt[i + j + 1]);
                    j += 2;
                } else {
                    j += 2;
                }
            }

            return rules;
        }

        static string GetRobotRuleName(int ruleType) {
            string ruleName;
            switch (ruleType) {
                case 1:
                    ruleName = "Disallow:";
                    break;

                case 2:
                    ruleName = "Allow:";
                    break;

                default:
                    ruleName = "INVALID";
                    break;
            }
            return ruleName;
        }

        static List<string> FetchRobotsTxt(string url) {
            string txt = "";
            List<string> organizedTxt = new List<string>();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK) {
                Stream responseStream = response.GetResponseStream();
                StreamReader reader;

                reader = new StreamReader(responseStream, Encoding.GetEncoding(response.CharacterSet));
                txt = reader.ReadToEnd();
                response.Close();
                reader.Close();

                organizedTxt = txt.Split(null).ToList();
            }

            return organizedTxt;
        }
    }
}
