using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class GamesTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating games table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE games (" +
                                      "GameKey CHAR(36) NOT NULL," +
                                      "CompetitionKey CHAR(36) NOT NULL," +
                                      "OrderBy INT NOT NULL," +
                                      "WinnerPlayerKey CHAR(36) NOT NULL," +
                                      "WinnerScore VARCHAR(100) NOT NULL," +
                                      "LoserPlayerKey CHAR(36) NOT NULL," +
                                      "LoserScore VARCHAR(100) NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (GameKey));";

                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE games " +
                                      "ADD INDEX CompetitionKey (CompetitionKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE games " +
                                      "ADD CONSTRAINT fk_competitions_games_CompetitionKey " +
                                      "FOREIGN KEY(CompetitionKey) " +
                                      "REFERENCES competitions (CompetitionKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE games " +
                                      "ADD INDEX OrderBy (CompetitionKey ASC, OrderBy ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE games " +
                                      "ADD CONSTRAINT fk_competition_players_games_WinnerPlayerKey " +
                                      "FOREIGN KEY (WinnerPlayerKey) " +
                                      "REFERENCES competition_players (CompetitionPlayerKey)" +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE games " +
                                      "ADD INDEX LoserPlayerKey (LoserPlayerKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE games " +
                                      "ADD CONSTRAINT fk_competition_players_games_LoserPlayerKey " +
                                      "FOREIGN KEY (LoserPlayerKey) " +
                                      "REFERENCES competition_players (CompetitionPlayerKey)" +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();
            }
        }
    }
}