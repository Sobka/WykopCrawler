using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using static Crawler.DatabaseControls;
using System.Threading;

namespace Crawler
{
    class Program
    {
        private static readonly string databaseName = "Wykop";
        private static readonly string firstPage = "https://www.wykop.pl/";
        
        private static readonly string containerXpath = ".//ul[@id='itemsStream']/li[@class='link iC ']";
        
        static void Main(string[] args)
        {
            // Define database controls, create tables, indices
            DatabaseControls databaseControls = new DatabaseControls();
            databaseControls.CreateDatabase(databaseName);
            databaseControls.ManageConnection(ConnectionControls.Open);
            databaseControls.CreateTable(databaseControls.createMainTable);
            databaseControls.CreateTable(databaseControls.createTagsTable);
            databaseControls.CreateTable(databaseControls.createCommentsTable);
            databaseControls.SetIndex("Main", "idx_main", "id");
            databaseControls.SetIndex("Tags", "idx_tags", "id");

            // Define node manager
            NodeManager nodeManager = new NodeManager();           
            
            // Get number of all pages
            int pages = nodeManager.GetNumOfPages(firstPage, ".//div[@class='wblock rbl-block pager']/p/a[@class='button']");

            int index = 1;
            int page = 1;
            int nullReferences = 0;
            int skipped = 0;

            // Loop over all pages
            for (int i = 1; i < 10; i++)
            {
                // Try-catch block to avoid getting NullReferenceExceptions 
                try
                {
                    // Define node collections, load pages
                    HtmlDocument htmlDocument = nodeManager.LoadPage($"https://www.wykop.pl/strona/{i}");
                    HtmlNodeCollection nodeCollection = nodeManager.GetNodeCollection(htmlDocument, containerXpath);

                    Console.WriteLine($"----------{page}----------");
                    // Iterate over every post container in collection and define post info
                    foreach (HtmlNode node in nodeCollection)
                    {
                        string id = Guid.NewGuid().ToString();
                        string title = nodeManager.Escape(nodeManager.GetInnerTextValue(node, ".//div[@class='lcontrast m-reset-margin']/h2/a").Trim());
                        int diggs = nodeManager.GetIntValue(node, ".//div[@class='diggbox ']//a//span");
                        string username = nodeManager.GetInnerTextValue(node, ".//div[@class='fix-tagline']/a").Replace("@", "");
                        string source = nodeManager.GetInnerTextValue(node, ".//span[@class='tag create'][1]");
                        int comments = nodeManager.GetIntValue(node, ".//div[@class='row elements']/a");
                        string description = nodeManager.Escape(nodeManager.GetInnerTextValue(node, ".//div[@class='description']/p/a").Trim());
                        DateTime date = nodeManager.GetDate(node, ".//span[@class='affect']/time", "title");

                        if (databaseControls.IsInTable(title))
                        {
                            Console.WriteLine($"{index}. Duplicate post - skipped");
                            index++;
                            continue;
                        }

                        // Skip all posts by Wykop Poleca
                        if (username.Equals("Wykop Poleca"))
                        {
                            Console.WriteLine($"{index}. Wykop Poleca - post skipped!");
                            skipped++;
                            index++;
                            continue;       
                        }

                        // Get tags
                        string[] tags = nodeManager.GetTags(node, ".//div[@class='fix-tagline']/a[contains(@class, 'tag affect ')]");

                        // Get link to the post page and load it
                        string link = nodeManager.GetAttribute(node, ".//div[@class='lcontrast m-reset-margin']/h2/a", "href");
                        htmlDocument = nodeManager.LoadPage(link);

                        // Get all post on post page
                        nodeCollection = nodeManager.GetNodeCollection(htmlDocument, ".//ul[@class='comments-stream']/li[@class='iC']");

                        // Console output, mostly for debugging
                        Console.WriteLine($"{index}. {title}");
                        index++;

                        // Iterate over every op post in comments page
                        foreach (HtmlNode opCommentNode in nodeCollection)
                        {
                            int opIsOp = 1;
                            string opUsername = nodeManager.GetInnerTextValue(opCommentNode, ".//div[contains(@class, 'wblock lcontrast')]//div[contains(@class, 'author ellipsis')]/a[1]/b");
                            int opPluses = nodeManager.GetIntValue(opCommentNode, ".//p[@class='vC']");
                            string opComment = nodeManager.GetInnerTextValue(opCommentNode, ".//div[@class='text ']/p").Trim();
                            string opVia = nodeManager.GetInnerTextValue(opCommentNode, ".//small[@class='affect']/span/a");
                            DateTime opDate = nodeManager.GetDate(opCommentNode, ".//small[@class='affect']/time", "title");
                            Console.WriteLine($"  |-->{opUsername}, {opDate}");

                            // Get all subposts for every post.
                            nodeCollection = nodeManager.GetNodeCollection(htmlDocument, ".//ul[@class='sub']/li");

                            // Iterate over all subposts
                            //foreach (HtmlNode commentNode in nodeCollection)
                            //{
                            //    int commentIsOp = 0;
                            //    string commentUsername = nodeManager.GetInnerTextValue(commentNode, ".//div[contains(@class, 'author ellipsis')]/a[1]/b");
                            //    Console.WriteLine($"  |----->{commentUsername}");
                            //}
                        }

                        // Insert data into database
                        databaseControls.InsertIntoMain(id, title, diggs, username, source, comments, description, date);
                        databaseControls.InsertIntoTags(id, tags[0], tags[1], tags[2], tags[3], tags[4], tags[5], tags[6], tags[7], tags[8], tags[9], tags[10]);            
                    }
                }

                catch (NullReferenceException)
                {
                    nullReferences++;
                    Console.WriteLine($"NullReferenceException, total: {nullReferences}");
                }
                page++;

            }
            // Close database connection
            Console.WriteLine("");
            Console.WriteLine($"Total errors: {nullReferences}");
            Console.WriteLine($"Total post skipped: {skipped}");

            databaseControls.ManageConnection(ConnectionControls.Close);
        }
    }
}
