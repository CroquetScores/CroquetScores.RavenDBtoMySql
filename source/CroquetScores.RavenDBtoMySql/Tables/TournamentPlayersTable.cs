using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentPlayersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating TournamentPlayers table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE TournamentPlayers (" +
                                      "TournamentPlayerKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "Name VARCHAR(100) NOT NULL," +
                                      "Representing VARCHAR(100)," +
                                      "Slug VARCHAR(100) NOT NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "RavenDbKey INT," +
                                      "PRIMARY KEY (TournamentPlayerKey));";

                command.ExecuteNonQuery();
            }
        }
    }
}