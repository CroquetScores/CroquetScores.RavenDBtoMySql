using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class CompetitionPlayersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating competition_players table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE competition_players (" +
                                      "CompetitionPlayerKey CHAR(36) NOT NULL," +
                                      "CompetitionKey CHAR(36) NOT NULL," +
                                      "PlayerKey CHAR(36) NOT NULL," +
                                      "OrderBy INT NOT NULL," +
                                      "Representing VARCHAR(100)," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "RavenDbKey INT," +
                                      "PRIMARY KEY (CompetitionPlayerKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE competition_players " +
                                      "ADD INDEX PlayerKey (PlayerKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE competition_players " +
                                      "ADD CONSTRAINT fk_players_competition_players_PlayerKey " +
                                      "FOREIGN KEY(PlayerKey) " +
                                      "REFERENCES players (PlayerKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE competition_players " +
                                      "ADD INDEX CompetitionKey (CompetitionKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE competition_players " +
                                      "ADD CONSTRAINT fk_competitions_competition_players_CompetitionKey " +
                                      "FOREIGN KEY(CompetitionKey) " +
                                      "REFERENCES competitions (CompetitionKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();
            }
        }
    }
}