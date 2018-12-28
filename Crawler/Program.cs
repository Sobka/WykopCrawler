using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using static Crawler.DatabaseControls;

namespace Crawler
{
    class Program
    {
        private static readonly string databaseName = "Wykop";
        private static readonly string firstPage = "https://www.wykop.pl/";
        private static readonly string createMainTable = "CREATE TABLE IF NOT EXISTS Main (" +
            "id TEXT," +
            "title TEXT," +
            "diggs NUMBER," +
            "username TEXT," +
            "source TEXT," +
            "comments NUMBER," +
            "description TEXT," +
            "date DATE);";
        private static readonly string createTagsTable = "CREATE TABLE IF NOT EXISTS Tags (" +
            "id TEXT," +
            "tag1 TEXT," +
            "tag2 TEXT," +
            "tag3 TEXT," +
            "tag4 TEXT," +
            "tag5 TEXT," +
            "tag6 TEXT," +
            "tag7 TEXT," +
            "tag8 TEXT," +
            "tag9 TEXT," +
            "tag10 TEXT," +
            "tag11 TEXT);";
        public static readonly string containerXpath = ".//ul[@id='itemsStream']/li[@class='link iC ']";

        static void Main(string[] args)
        {
            // Define database controls, create tables
            DatabaseControls databaseControls = new DatabaseControls();
            databaseControls.CreateDatabase(databaseName);
            databaseControls.ManageConnection(ConnectionControls.Open);
            databaseControls.CreateTable(createMainTable);
            databaseControls.CreateTable(createTagsTable);

            // Define node manager
            NodeManager nodeManager = new NodeManager();           
            
            // Get number of all pages
            int pages = nodeManager.GetNumOfPages(firstPage, ".//div[@class='wblock rbl-block pager']/p/a[@class='button']");

            int index = 1;
            int page = 1;
            int nullReferences = 0;
            int skipped = 0;

            // Loop over all pages
            for (int i = 1; i < 20; i++)
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
                        string title = nodeManager.Escape(nodeManager.GetGenericValue(node, ".//div[@class='lcontrast m-reset-margin']/h2/a").Trim());
                        int diggs = nodeManager.GetIntValue(node, ".//div[@class='diggbox ']//a//span");
                        string username = nodeManager.GetGenericValue(node, ".//div[@class='fix-tagline']/a").Replace("@", "");
                        string source = nodeManager.GetGenericValue(node, ".//span[@class='tag create'][1]");
                        int comments = nodeManager.GetIntValue(node, ".//div[@class='row elements']/a");
                        string description = nodeManager.Escape(nodeManager.GetGenericValue(node, ".//div[@class='description']/p/a").Trim());
                        DateTime date = nodeManager.GetDate(node, ".//span[@class='affect']/time", "title");

                        // Skip all posts by Wykop Poleca
                        if (username.Equals("Wykop Poleca"))
                        {
                            Console.WriteLine($"{index}. Wykop Poleca - post skipped!");
                            skipped++;
                            continue;       
                        }

                        string[] tags = nodeManager.GetTags(node, ".//div[@class='fix-tagline']/a[contains(@class, 'tag affect ')]");

                        string tag1 = tags[0];
                        string tag2 = tags[1];
                        string tag3 = tags[2];
                        string tag4 = tags[3];
                        string tag5 = tags[4];
                        string tag6 = tags[5];
                        string tag7 = tags[6];
                        string tag8 = tags[7];
                        string tag9 = tags[8];
                        string tag10 = tags[9];
                        string tag11 = tags[10];

                        databaseControls.InsertIntoTags(id, tag1, tag2, tag3, tag4, tag5, tag6, tag7, tag8, tag9, tag10, tag11);

                        // Insert post info into database
                        databaseControls.InsertIntoMain(id, title, diggs, username, source, comments, description, date);

                        // Console output, mostly for debugging
                        Console.WriteLine($"{index}. {title}");
                        index++;
                    }
                }
                catch (NullReferenceException)
                {
                    nullReferences++;
                    Console.WriteLine($"NullReferenceException, total: {nullReferences}");
                }
                page++;
                //something new
            }
            // Close database connection
            Console.WriteLine("");
            Console.WriteLine($"Total errors: {nullReferences}");
            Console.WriteLine($"Total post skipped: {skipped}");
            databaseControls.ManageConnection(ConnectionControls.Close);
        }
    }
}
