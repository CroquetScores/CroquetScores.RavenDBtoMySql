using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class TournamentsTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating Tournaments table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Tournaments (" +
                                      "TournamentKey CHAR(36) NOT NULL, " +
                                      "Site VARCHAR(50) NOT NULL, " +
                                      "Name VARCHAR(600) NOT NULL, " +
                                      "Slug VARCHAR(600) NOT NULL, " +
                                      "Start DATETIME NOT NULL, " +
                                      "Finish DATETIME NOT NULL, " +
                                      "CreateBy_UserKey CHAR(36) NOT NULL, " +
                                      "IsArchived BOOLEAN NOT NULL, " +
                                      "IsMajorTournament BOOLEAN NOT NULL, " +
                                      "SportType VARCHAR(3) NOT NULL, " +
                                      "TournamentType VARCHAR(20) NOT NULL, " +
                                      "TimeZoneId VARCHAR(50) NOT NULL, " +
                                      "Created DATETIME NOT NULL, " +
                                      "LastUpdate DATETIME NOT NULL, " +
                                      "PRIMARY KEY (TournamentKey));";
                command.ExecuteNonQuery();
            }
        }
    }
}