using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class MySqlTables
    {
        public static void Create(MySqlConnection connection)
        {
            UsersTable.CreateTable(connection);
            PlayersTable.CreateTable(connection);
            TournamentsTable.CreateTable(connection); // Must be after Users
            CompetitionsTable.CreateTable(connection); // Must be after Tournaments
            CompetitionPlayersTable.CreateTable(connection); // Must be after Competitions and Players
            GamesTable.CreateTable(connection); // Must be after Competitions and Players
            TournamentManagersTable.CreateTable(connection); // Must be after Tournaments and Users
            TournamentScorersTable.CreateTable(connection); // Must be after Tournaments and Users
        }
    }
}