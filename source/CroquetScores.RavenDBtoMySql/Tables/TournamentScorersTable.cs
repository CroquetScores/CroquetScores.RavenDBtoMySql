using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentScorersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating tournament_scorers table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE tournament_scorers (" +
                                      "TournamentScorerKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "UserKey CHAR(36) NOT NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (TournamentScorerKey));";
                command.ExecuteNonQuery();
            }
        }
    }
}