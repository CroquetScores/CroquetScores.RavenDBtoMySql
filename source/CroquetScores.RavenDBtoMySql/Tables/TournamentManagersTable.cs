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
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (TournamentManagerKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_managers " +
                                      "ADD INDEX TournamentKey (TournamentKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_managers " +
                                      "ADD CONSTRAINT fk_tournaments_tournament_managers_TournamentKey " +
                                      "FOREIGN KEY (TournamentKey) " +
                                      "REFERENCES tournaments (TournamentKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_managers " +
                                      "ADD INDEX UserKey (UserKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE tournament_managers " +
                                      "ADD CONSTRAINT fk_users_tournament_managers_UserKey " +
                                      "FOREIGN KEY (UserKey) " +
                                      "REFERENCES users (UserKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();
            }
        }
    }
}