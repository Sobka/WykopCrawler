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
        public readonly string createMainTable = "CREATE TABLE IF NOT EXISTS Main (" +
            "id TEXT," +
            "title TEXT," +
            "diggs NUMBER," +
            "username TEXT," +
            "source TEXT," +
            "comments NUMBER," +
            "description TEXT," +
            "date DATE);";
        public readonly string createTagsTable = "CREATE TABLE IF NOT EXISTS Tags (" +
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
        public readonly string createCommentsTable = "CREATE TABLE IF NOT EXISTS Comments (" +
            "id TEXT," +
            "commentId TEXT," +
            "isOP NUMBER," + // 1 if yes 0 if no
            "username TEXT," +
            "pluses NUMBER," +
            "comment TEXT," +
            "via TEXT," +
            "date DATE);";

        public enum ConnectionControls
        {
            Open,
            Close
        }

        public SQLiteConnection Connection { get; private set; }

        public void CreateDatabase(string databaseName)
        {
            if (!databaseName.EndsWith(".db"))
            {
                databaseName += ".db";
            }
            Connection = new SQLiteConnection($"Data Source=..\\..\\{databaseName}");
        }

        public void ManageConnection(ConnectionControls controls)
        {
            switch (controls)
            {
                case ConnectionControls.Open:
                    Connection.Open();
                    Console.WriteLine("Opening connection to the database...");
                    break;
                case ConnectionControls.Close:
                    Connection.Close();
                    Console.WriteLine("Closing connection to the database...");
                    break;
            }
        }

        public void CreateTable(string createTableSQL)
        {
            SQLiteCommand command = new SQLiteCommand(createTableSQL, Connection);
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

            SQLiteCommand command = new SQLiteCommand(insertSQL, Connection);
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

            SQLiteCommand command = new SQLiteCommand(insertSQL, Connection);
            command.Parameters.AddRange(parameters);
            command.ExecuteNonQuery();
        }  

        public void InsertIntoComments(string id, string commentId, int isOP, string username, int pluses, string comment, string via, DateTime date)
        {
            using (SQLiteCommand command = Connection.CreateCommand())
            {
                SQLiteParameter[] parameters =
                {
                    new SQLiteParameter("@id", id),
                    new SQLiteParameter("@commentId", commentId),
                    new SQLiteParameter("@isOP", isOP),
                    new SQLiteParameter("@username", username),
                    new SQLiteParameter("@pluses", pluses),
                    new SQLiteParameter("@comment", comment),
                    new SQLiteParameter("@via", via),
                    new SQLiteParameter("@date", date)
                };
                command.CommandText = "INSERT INTO Comments (id, commentId, isOP, username, pluses, comment, via, date) VALUES (@id, @commentId, @isOP, @username, @pluses, @comment, @via, @date);";
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }
        
        public void SetIndex(string tableName, string indexName, string indexColumn)
        {
            string indexSQL = $"CREATE  INDEX IF NOT EXISTS {indexName} ON {tableName}({indexColumn});";
            SQLiteCommand command = new SQLiteCommand(indexSQL, Connection);
            command.ExecuteNonQuery();
        }

        public bool IsInComments(string comment, string commentId, string id)
        {
            bool found = false;
            string selectSQL = $"SELECT id, commentId, comment FROM Comments WHERE comment = @comment";
            using (SQLiteCommand command = Connection.CreateCommand()) 
            {
                command.CommandText = selectSQL;
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.Add(new SQLiteParameter("@comment", comment));
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetString(2).Equals(comment))
                        {
                            found = true;
                            id = reader.GetString(0);
                            commentId = reader.GetString(1);
                            break;
                        }
                    }
                }
            }
            return found;          
        }

        public bool IsInMain(string title)
        {
            bool found = false;
            string sql = $"SELECT title FROM Main WHERE title = @title";
            using (SQLiteCommand command = Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.Add(new SQLiteParameter("@title", title));
                using (SQLiteDataReader r = command.ExecuteReader())
                {
                    while (r.Read())
                    {
                        if (r.GetString(0).Equals(title))
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }
            return found;

        }



    }
}
