using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class DatabaseControls
    {
        public SQLiteConnection connection { get; private set; }

        public void CreateDatabase(string databaseName)
        {
            if (!databaseName.EndsWith(".db"))
            {
                databaseName += ".db";
            }
            connection = new SQLiteConnection($"Data Source=..\\..\\{databaseName}");
        }

        public void ManageConnection(string option)
        {
            switch (option)
            {
                case "open":
                    connection.Open();
                    Console.WriteLine($"Oppening connection to the database...");
                    break;
                case "close":
                    connection.Close();
                    Console.WriteLine("Closeing connection to the database...");
                    break;
            }
        }

        public void CreateTable(string createTableSQL)
        {
            SQLiteCommand command = new SQLiteCommand(createTableSQL, connection);
            command.ExecuteNonQuery();
        }
    }
}
