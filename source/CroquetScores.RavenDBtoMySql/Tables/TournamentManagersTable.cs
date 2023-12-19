using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentManagersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating tournament_managers table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE tournament_managers (" +
                                      "TournamentManagerKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "UserKey CHAR(36) NOT NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (TournamentManagerKey));";
                command.ExecuteNonQuery();
            }
        }
    }
}