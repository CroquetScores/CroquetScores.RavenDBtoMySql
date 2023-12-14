using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class MySqlTables
    {
        public static void Create(MySqlConnection connection)
        {
            UsersTable.CreateTable(connection);
            TournamentsTable.CreateTable(connection);
            TournamentManagersTable.CreateTable(connection);
            TournamentScorersTable.CreateTable(connection);
        }
    }
}