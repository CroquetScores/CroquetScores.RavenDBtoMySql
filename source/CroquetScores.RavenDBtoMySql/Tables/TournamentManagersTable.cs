using System;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentManagersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Console.WriteLine("Creating TournamentManagers table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE TournamentManagers (" +
                                      "TournamentManagerKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "UserKey CHAR(36) NOT NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (TournamentManagerKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE TournamentManagers" +
                                      " ADD INDEX TournamentKey (TournamentKey ASC) VISIBLE;";
                command.ExecuteNonQuery();
            }
        }
    }
}