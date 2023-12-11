using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Support
{
    internal class MySqlSupport
    {
        public static void CreateDatabase()
        {
            using (var connection = OpenConnection())
            {
                DropDatabaseIfExists(connection);
                CreateDatabase(connection);
            }
        }

        private static MySqlConnection OpenConnection()
        {
            var connectionString = $"server={ConfigurationManager.AppSettings["MySql:server"]};" +
                                   $"user={ConfigurationManager.AppSettings["MySql:User"]};" +
                                   $"password={ConfigurationManager.AppSettings["MySql:Password"]};";
            
            var connection = new MySqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        private static void DropDatabaseIfExists(MySqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "drop schema if exists CroquetScores";
            command.ExecuteNonQuery();
        }

        private static void CreateDatabase(MySqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "create schema CroquetScores";
            command.ExecuteNonQuery();
        }
    }
}