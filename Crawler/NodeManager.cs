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

        public string GetInnerTextValue(HtmlNode node, string xpath)
        {
            string value = "";
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
                if (defaultString.Contains("skomentuj"))
                {
                    return value;
                }
                Regex re = new Regex(@"-?\d+");
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

        public int GetNumOfPages(string url, string xpath)
        {
            int index = LoadPage(url).DocumentNode.SelectNodes(xpath).Count;
            string pages = LoadPage(url).DocumentNode.SelectNodes(xpath)[index-2].InnerText;
            return Int32.Parse(pages);
        }

        public string[] GetTags(HtmlNode node, string xpath)
        {
            string[] tags = new string[11];
            HtmlNodeCollection nodes = node.SelectNodes(xpath);
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] != null)
                {
                    tags[i] = nodes[i].InnerText.Replace("#", "");
                }
            }
            return tags;
        }

        public string GetAttribute(HtmlNode node, string xpath, string attribute)
        {
            string returnAttribute = "";
            if (node.SelectSingleNode(xpath) != null)
            {
                returnAttribute = node.SelectSingleNode(xpath).GetAttributeValue(attribute, "");
            }
            return returnAttribute;
        }
    }

    
}
