using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Crawler
{
    class NodeManager
    {
        public HtmlDocument LoadPage(string url)
        {
            return new HtmlWeb().Load(url);
        }

        public HtmlNodeCollection GetNodeCollection(HtmlDocument htmlDocument, string xpath)
        {
            HtmlNodeCollection collection = htmlDocument.DocumentNode.SelectNodes(xpath);
            return collection;
        }

        public string GetGenericValue(HtmlNode node, string xpath)
        {
            string value = "Brak danych";
            if (node.SelectSingleNode(xpath) != null)
            {
                value = node.SelectSingleNode(xpath).InnerText;
            }
            return value;
        }

        public int GetIntValue(HtmlNode node, string xpath)
        {
            int value = 0;
            if (node.SelectSingleNode(xpath) != null)
            {
                string defaultString = node.SelectSingleNode(xpath).InnerText;
                Regex re = new Regex(@"\d+");
                Match match = re.Match(defaultString);
                value = Int32.Parse(match.ToString());
            }
            return value;
        }

        public string Escape(string stringToBeReplaced)
        {        
            return stringToBeReplaced.Replace("&amp", "&").Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&gt;", ">").Replace("&lt;", "<");
        }

        public DateTime GetDate(HtmlNode node, string xpath, string attribute)
        {
            DateTime date = new DateTime(9999, 9, 9, 9, 9, 9);
            if (node.SelectSingleNode(xpath) != null)
            {
                date = DateTime.Parse(node.SelectSingleNode(xpath).GetAttributeValue(attribute, "9999-99-99 99:99:99"));
            }
            return date;
        }
    }

    
}
