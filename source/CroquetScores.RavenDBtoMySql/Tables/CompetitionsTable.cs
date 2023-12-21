using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class CompetitionsTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating competitions table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE competitions (" +
                                      "CompetitionKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "OrderBy INT NOT NULL," +
                                      "Name VARCHAR(200) NOT NULL," +
                                      "Slug VARCHAR(200) NOT NULL," +
                                      "Type VARCHAR(10) NOT NULL," +
                                      "TypeProperties JSON NOT NULL," +
                                      "IsArchived BOOLEAN NOT NULL," +
                                      "RavenDbKey VARCHAR(50) NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (CompetitionKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE competitions " +
                                      "ADD INDEX TournamentKey (TournamentKey ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE competitions " +
                                      "ADD CONSTRAINT fk_tournaments_competitions_TournamentKey " +
                                      "FOREIGN KEY (TournamentKey) " +
                                      "REFERENCES tournaments (TournamentKey) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();
            }
        }
    }
}