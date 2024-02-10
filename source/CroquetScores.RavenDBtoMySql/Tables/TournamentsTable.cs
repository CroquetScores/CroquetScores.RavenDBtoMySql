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
                                      "Site VARCHAR(300) NOT NULL, " +
                                      "TournamentKey CHAR(36) NOT NULL, " +
                                      "Name VARCHAR(200) NOT NULL, " +
                                      "Slug VARCHAR(200) NOT NULL, " +
                                      "Starts DATETIME NOT NULL, " +
                                      "Finishes DATETIME NOT NULL, " +
                                      "IsArchived BOOLEAN NOT NULL, " +
                                      "IsMajorTournament BOOLEAN NOT NULL, " +
                                      "SportType VARCHAR(3) NOT NULL, " +
                                      "TournamentType VARCHAR(20) NOT NULL, " +
                                      "TimeZoneInfo VARCHAR(50) NOT NULL, " +
                                      "LastUpdate DATETIME NOT NULL, " +
                                      "PRIMARY KEY (TournamentKey));";
                command.ExecuteNonQuery();
            }
        }
    }
}