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

                command.CommandText = "ALTER TABLE tournament_scorers " +
                                      "ADD INDEX TournamentKey (TournamentKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_scorers " +
                                      "ADD CONSTRAINT fk_tournaments_tournament_scorers_TournamentKey " +
                                      "FOREIGN KEY (TournamentKey) " +
                                      "REFERENCES tournaments (TournamentKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_scorers " +
                                      "ADD INDEX UserKey (UserKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_scorers " +
                                      "ADD CONSTRAINT fk_users_tournament_scorers_UserKey " +
                                      "FOREIGN KEY (UserKey) " +
                                      "REFERENCES users (UserKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();
            }
        }
    }
}