using System.Configuration;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Support
{
    internal class MySqlSupport
    {
        public static MySqlConnection OpenConnection()
        {
            var connectionString = $"server={ConfigurationManager.AppSettings["MySql:server"]};" +
                                   $"user={ConfigurationManager.AppSettings["MySql:User"]};" +
                                   $"password={ConfigurationManager.AppSettings["MySql:Password"]};";
            
            var connection = new MySqlConnection(connectionString);

            connection.Open();

            return connection;
        }

        public static void DropDatabaseIfExists(MySqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "drop schema if exists CroquetScores";
            command.ExecuteNonQuery();
        }

        public static void CreateDatabase(MySqlConnection connection)
        {
            var command = connection.CreateCommand();
            command.CommandText = "create schema CroquetScores";
            command.ExecuteNonQuery();
        }
    }
}