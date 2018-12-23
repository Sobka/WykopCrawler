using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
