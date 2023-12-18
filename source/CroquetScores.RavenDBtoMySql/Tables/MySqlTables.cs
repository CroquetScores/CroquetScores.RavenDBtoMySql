using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class MySqlTables
    {
        public static void Create(MySqlConnection connection)
        {
            CompetitionsTable.CreateTable(connection);
            GamesTable.CreateTable(connection);
            UsersTable.CreateTable(connection);
            TournamentsTable.CreateTable(connection);
            TournamentManagersTable.CreateTable(connection);
            TournamentPlayersTable.CreateTable(connection);
            TournamentScorersTable.CreateTable(connection);
        }
    }
}