using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class GamesTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating Games table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Games (" +
                                      "GameKey CHAR(36) NOT NULL," +
                                      "CompetitionKey CHAR(36) NOT NULL," +
                                      "OrderBy INT NOT NULL," +
                                      "WinnerPlayerKey CHAR(36) NOT NULL," +
                                      "WinnerScore VARCHAR(100) NOT NULL," +
                                      "LoserPlayerKey CHAR(36) NOT NULL," +
                                      "LoserScore VARCHAR(100) NOT NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (GameKey));";

                command.ExecuteNonQuery();
            }
        }
    }
}