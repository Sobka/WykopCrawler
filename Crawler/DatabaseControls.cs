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

        public enum ConnectionControls
        {
            Open,
            Close
        }

        public SQLiteConnection connection { get; private set; }

        public void CreateDatabase(string databaseName)
        {
            if (!databaseName.EndsWith(".db"))
            {
                databaseName += ".db";
            }
            connection = new SQLiteConnection($"Data Source=..\\..\\{databaseName}");
        }

        public void ManageConnection(ConnectionControls controls)
        {
            switch (controls)
            {
                case ConnectionControls.Open:
                    connection.Open();
                    Console.WriteLine($"Oppening connection to the database...");
                    break;
                case ConnectionControls.Close:
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

        public void InsertIntoDatabase(string id, string title, int diggs, string username, string source, int comments, string description, DateTime date)
        {
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@id", id),
                new SQLiteParameter("@title", title),
                new SQLiteParameter("@diggs", diggs),
                new SQLiteParameter("@username", username),
                new SQLiteParameter("@source", source),
                new SQLiteParameter("@comments", comments),
                new SQLiteParameter("@description", description),
                new SQLiteParameter("@date", date)
            };
            string insertSQL = "INSERT INTO Main (id, title, diggs, username, source, comments, description, date) "
                + "VALUES (@id, @title, @diggs, @username, @source, @comments, @description, @date)";

            SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
            command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }
 
    }
}
