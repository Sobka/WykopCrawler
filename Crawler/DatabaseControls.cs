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
                    Console.WriteLine("Opening connection to the database...");
                    break;
                case ConnectionControls.Close:
                    connection.Close();
                    Console.WriteLine("Closing connection to the database...");
                    break;
            }
        }

        public void CreateTable(string createTableSQL)
        {
            SQLiteCommand command = new SQLiteCommand(createTableSQL, connection);
            command.ExecuteNonQuery();
        }

        public void InsertIntoMain(string id, string title, int diggs, string username, string source, int comments, string description, DateTime date)
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

        public void InsertIntoTags(string id, string tag1, string tag2, string tag3, string tag4, string tag5, string tag6, string tag7, string tag8, string tag9, string tag10, string tag11)
        {
            SQLiteParameter[] parameters =
            {
                new SQLiteParameter("@id", id),
                new SQLiteParameter("@tag1", tag1),
                new SQLiteParameter("@tag2", tag2),
                new SQLiteParameter("@tag3", tag3),
                new SQLiteParameter("@tag4", tag4),
                new SQLiteParameter("@tag5", tag5),
                new SQLiteParameter("@tag6", tag6),
                new SQLiteParameter("@tag7", tag7),
                new SQLiteParameter("@tag8", tag8),
                new SQLiteParameter("@tag9", tag9),
                new SQLiteParameter("@tag10", tag10),
                new SQLiteParameter("@tag11", tag11)
            };

            string insertSQL = "INSERT INTO Tags (id, tag1, tag2, tag3, tag4, tag5, tag6, tag7, tag8, tag9, tag10, tag11) VALUES (@id, @tag1, @tag2, @tag3, @tag4, @tag5, @tag6, @tag7, @tag8, @tag9, @tag10, @tag11)";

            SQLiteCommand command = new SQLiteCommand(insertSQL, connection);
            command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }  
        
        public void SetIndex(string tableName, string indexName, string indexColumn)
        {
            //CREATE[UNIQUE] INDEX index_name ON table_name(indexed_column);
            string indexSQL = $"CREATE UNIQUE INDEX {indexName} ON {tableName}({indexColumn});";
            SQLiteCommand command = new SQLiteCommand(indexSQL, connection);
            command.ExecuteNonQuery();
        }
    }
}
