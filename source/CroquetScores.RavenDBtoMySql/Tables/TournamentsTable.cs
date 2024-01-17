using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentsTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating tournaments table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE tournaments (" +
                                      "TournamentKey CHAR(36) NOT NULL, " +
                                      "Site VARCHAR(50) NOT NULL, " +
                                      "Name VARCHAR(200) NOT NULL, " +
                                      "Slug VARCHAR(200) NOT NULL, " +
                                      "Starts DATETIME NOT NULL, " +
                                      "Finishes DATETIME NOT NULL, " +
                                      "CreatedBy_UserKey CHAR(36) NOT NULL, " +
                                      "IsArchived BOOLEAN NOT NULL, " +
                                      "IsMajorTournament BOOLEAN NOT NULL, " +
                                      "SportType VARCHAR(3) NOT NULL, " +
                                      "TournamentType VARCHAR(20) NOT NULL, " +
                                      "TimeZoneInfo VARCHAR(50) NOT NULL, " +
                                      "Created DATETIME NOT NULL, " +
                                      "LastUpdate DATETIME NOT NULL, " +
                                      "PRIMARY KEY (TournamentKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE `croquetscores`.`tournaments` " +
                                      "ADD INDEX `fk_users_tournaments_CreatedBy_UserKey_idx` (`CreatedBy_UserKey` ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE `croquetscores`.`tournaments` " +
                                      "ADD CONSTRAINT `fk_users_tournaments_CreatedBy_UserKey` " +
                                      "FOREIGN KEY (`CreatedBy_UserKey`)" +
                                      "REFERENCES `croquetscores`.`users` (`UserKey`) " +
                                      "ON DELETE NO ACTION " +
                                      "ON UPDATE NO ACTION;";
                command.ExecuteNonQuery();
            }
        }
    }
}