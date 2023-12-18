using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class CompetitionsTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating Competitions table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Competitions (" +
                                      "CompetitionKey CHAR(36) NOT NULL," +
                                      "TournamentKey CHAR(36) NOT NULL," +
                                      "Name VARCHAR(100) NOT NULL," +
                                      "Slug VARCHAR(100) NOT NULL," +
                                      "Type VARCHAR(10) NOT NULL," +
                                      "TypeProperties JSON NOT NULL," +
                                      "IsArchived BOOLEAN NOT NULL," +
                                      "RavenDbKey VARCHAR(50) NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (CompetitionKey));";
                command.ExecuteNonQuery();
            }
        }
    }
}