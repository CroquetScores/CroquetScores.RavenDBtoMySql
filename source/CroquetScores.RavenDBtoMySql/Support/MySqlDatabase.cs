using System.Configuration;
using CroquetScores.RavenDBtoMySql.Tables;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Support
{
    internal class MySqlDatabase
    {
        public static void CreateDatabase()
        {
            using (var connection = OpenConnection())
            {
                DropDatabaseIfExists(connection);
                CreateDatabase(connection);
            }

            using (var connection = OpenDatabase())
            {
                MySqlTables.Create(connection);
            }
        }

        public static MySqlConnection OpenDatabase()
        {
            var connectionString = GetConnectionStringWithDatabase();
            var connection = new MySqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        private static void CreateDatabase(MySqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "create schema CroquetScores";
            command.ExecuteNonQuery();
        }

        private static void DropDatabaseIfExists(MySqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "drop schema if exists CroquetScores";
            command.ExecuteNonQuery();
        }

        private static string GetConnectionStringWithDatabase()
        {
            return GetConnectionStringWithoutDatabase() +
                   $"database={ConfigurationManager.AppSettings["MySql:Database"]}";
        }

        private static string GetConnectionStringWithoutDatabase()
        {
            var connectionString = $"server={ConfigurationManager.AppSettings["MySql:server"]};" +
                                   $"user={ConfigurationManager.AppSettings["MySql:User"]};" +
                                   $"password={ConfigurationManager.AppSettings["MySql:Password"]};";

            return connectionString;
        }

        private static MySqlConnection OpenConnection()
        {
            var connectionString = GetConnectionStringWithoutDatabase();
            var connection = new MySqlConnection(connectionString);

            connection.Open();

            return connection;
        }
    }
}