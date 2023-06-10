using Microsoft.Data.Sqlite;
using P4_Vacation_photos.Models.DB.Primitives;

namespace P4_Vacation_photos.Models.DB.Providers
{
    public class SQLiteProvider : IDbProvider
    {
        SqliteConnection _connection;


        /// <summary>Constructor - Opens the connection to the database</summary>
        /// <param name="path">The path to the sqlite database</param>
        public SQLiteProvider(string path)
        {
            SqliteConnectionStringBuilder connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = path;
            _connection = new SqliteConnection(connectionStringBuilder.ToString());
            _connection.Open();
        }


        /// <summary>Destructor - Closes the connection to the database</summary>
        ~SQLiteProvider()
        {
            _connection.Close();
        }

        /// <summary>Gets a row(s) from the database</summary>
        /// <param name="table">The table to get the row from</param>
        /// <param name="columns">The columns to get</param>
        /// <param name="where">The where clause</param>
        /// <param name="limit">The limit of rows to get</param>
        /// <param name="offset">The offset of rows to get</param>
        public Row[] select(string table, string[]? columns = null, Where[]? where = null, int? limit = 1, int? offset = null)
        {
            List<Row> rows = new List<Row>();
            string query = $"SELECT {(columns != null && columns.Length > 0 ? $"{string.Join(", ", columns.Select(x => '`' + x + '`'))}" : "*")} FROM `{table}` {(where != null && where.Length > 0 ? $"WHERE {string.Join(" AND ", generateWhereClause(where))}" : "")} {(limit != null ? $"LIMIT {limit}" : "")} {(offset != null ? $"OFFSET {offset}" : "")} ";
            SqliteCommand command = new SqliteCommand(query, _connection);
            if (where != null)
                foreach (Where whereClause in where)
                {
                    command.Parameters.AddWithValue('@' + whereClause._column, whereClause._value);
                }
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                List<P4_Vacation_photos.Models.DB.Primitives.Column> columnsList = new List<P4_Vacation_photos.Models.DB.Primitives.Column>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (reader.IsDBNull(i))
                    {
                        columnsList.Add(new P4_Vacation_photos.Models.DB.Primitives.Column(reader.GetName(i), ""));
                    }
                    else
                    {
                        columnsList.Add(new P4_Vacation_photos.Models.DB.Primitives.Column(reader.GetName(i), reader.GetValue(i)));
                    }
                }
                rows.Add(new Row(columnsList));
            }
            return rows.ToArray();
        }

        /// <summary>Inserts a row(s) into the database</summary>
        /// <param name="table">The table to insert the row into</param>
        /// <param name="data">The data to insert</param>
        public bool insert(string table, List<P4_Vacation_photos.Models.DB.Primitives.Column> data)
        {
            string query = $"INSERT INTO `{table}` ({string.Join(", ", data.Select(x => '`' + x._column + '`'))}) VALUES ({string.Join(", ", data.Select(x => '@' + x._column))})";
            SqliteCommand command = new SqliteCommand(query, _connection);
            foreach (P4_Vacation_photos.Models.DB.Primitives.Column column in data)
            {
                command.Parameters.AddWithValue('@' + column._column, column._value);
            }
            return command.ExecuteNonQuery() > 0;
        }

        /// <summary>Updates a row(s) in the database</summary>
        /// <param name="table">The table to update the row in</param>
        /// <param name="data">The data to update</param>
        /// <param name="where">The where clause</param>
        /// <param name="limit">The limit of rows to update (only works if SQLITE_ENABLE_UPDATE_DELETE_LIMIT is added add compile time)</param>
        public bool update(string table, List<P4_Vacation_photos.Models.DB.Primitives.Column> data, Where[]? where = null, int limit = 1)
        {
            string query = $"UPDATE `{table}` SET {string.Join(", ", data.Select(x => '`' + x._column + "` = @" + x._column))} {(where != null && where.Length > 0 ? $"WHERE {string.Join(" AND ", generateWhereClause(where))}" : "")} {(limit > 0 ? $"LIMIT {limit}" : "")}";
            SqliteCommand command = new SqliteCommand(query, _connection);
            foreach (P4_Vacation_photos.Models.DB.Primitives.Column column in data)
            {
                command.Parameters.AddWithValue('@' + column._column, column._value);
            }
            if (where != null)
                foreach (Where whereClause in where)
                {
                    command.Parameters.AddWithValue('@' + whereClause._column, whereClause._value);
                }
            return limit > 0 ? command.ExecuteNonQuery() <= limit : command.ExecuteNonQuery() > 0;
        }
        /// <summary>Deletes a row(s) from the database</summary>
        /// <param name="table">The table to delete the row from</param>
        /// <param name="where">The where clause - HAS TO HAVE AT LEAST 1 WHERE CLAUSE</param>
        /// <param name="limit">The limit of rows to delete (only works if SQLITE_ENABLE_UPDATE_DELETE_LIMIT is added add compile time)</param>
        public bool delete(string table, Where[]? where = null, int limit = 1)
        {
            if (where == null || where.Length < 1)
            {
                throw new Exception("You must specify a where clause to delete from the database");
            }
            string query = $"DELETE FROM `{table}` WHERE {string.Join(" AND ", generateWhereClause(where))} {(limit > 0 ? $"LIMIT {limit}" : "")}";
            SqliteCommand command = new SqliteCommand(query, _connection);
            foreach (Where whereClause in where)
            {
                command.Parameters.AddWithValue('@' + whereClause._column, whereClause._value);
            }
            return limit > 0 ? command.ExecuteNonQuery() <= limit : command.ExecuteNonQuery() > 0;
        }
        /// <summary>Counts how many rows</summary>
        /// <param name="table">The table to count the rows from</param>
        /// <param name="where">The where clause</param>
        public int count(string table, Where[]? where = null)
        {
            string query = $"SELECT COUNT(*) FROM `{table}` {(where != null && where.Length > 0 ? $"WHERE {string.Join(" AND ", generateWhereClause(where))}" : "")}";
            SqliteCommand command = new SqliteCommand(query, _connection);
            if (where != null)
                foreach (Where whereClause in where)
                {
                    command.Parameters.AddWithValue('@' + whereClause._column, whereClause._value);
                }
            return Convert.ToInt32(command.ExecuteScalar());
        }
        /// <summary>Executes a raw query</summary>
        /// <param name="query">The query to execute</param>
        /// <param name="parameters">The parameters to add to the query</param>
        public Row[] rawQuery(string query, (string column, dynamic value)[]? parameters = null)
        {
            SqliteCommand command = new SqliteCommand(query, _connection);
            var execution = command.ExecuteReader();
            // parameters
            if (parameters != null)
                foreach ((string column, dynamic value) parameter in parameters)
                {
                    command.Parameters.AddWithValue('@' + parameter.column, parameter.value);
                }

            List<Row> rows = new List<Row>();
            while (execution.Read())
            {
                List<P4_Vacation_photos.Models.DB.Primitives.Column> columnsList = new List<P4_Vacation_photos.Models.DB.Primitives.Column>();
                for (int i = 0; i < execution.FieldCount; i++)
                {
                    if (execution.IsDBNull(i))
                    {
                        columnsList.Add(new P4_Vacation_photos.Models.DB.Primitives.Column(execution.GetName(i), ""));
                    }
                    else
                    {
                        columnsList.Add(new P4_Vacation_photos.Models.DB.Primitives.Column(execution.GetName(i), execution.GetValue(i)));
                    }
                }
                rows.Add(new Row(columnsList));
            }
            return rows.ToArray();
        }

        /// <summary>Generates the correct string for the where clause of the query</summary>
        private string[] generateWhereClause(Where[] where)
        {
            List<string> whereClauses = new List<string>();
            foreach (Where whereClause in where)
            {
                whereClauses.Add($"`{whereClause._column}` {new TransformCompare().change(whereClause._operator)} @{whereClause._column}");
            }
            return whereClauses.ToArray();
        }
    }
}