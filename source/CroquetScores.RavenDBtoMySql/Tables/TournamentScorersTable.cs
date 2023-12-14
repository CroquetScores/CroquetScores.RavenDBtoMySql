using System;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentScorersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Console.WriteLine("Creating TournamentScorers table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE TournamentScorers (" +
                                      "TournamentScorerKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "UserKey CHAR(36) NOT NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (TournamentScorerKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE TournamentScorers" +
                                      " ADD INDEX TournamentKey (TournamentKey ASC) VISIBLE;";
                command.ExecuteNonQuery();
            }
        }
    }
}