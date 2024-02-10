using System;
using System.Linq;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Support;
using CroquetScores.RavenDBtoMySql.Tables;
using MySql.Data.MySqlClient;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class TournamentsImporter
    {
        private static int _maxNameLength;
        private static int _maxTimeZoneInfoLength;

        public static void Import(IDocumentStore documentStore, MySqlConnection connection, string site)
        {
            Log.Progress("Importing Tournaments data...");

            int totalCount;
            var skip = 0;
            const int take = 100;
            var moreToRead = true;

            using (var session = documentStore.OpenSession())
            {
                totalCount = session.Query<Tournament>().Count(x => !x.IsArchived);
            }

            Log.Statistic($"{totalCount:N0} {site} tournaments to import...");

            using (var command = CreateInsertCommand(connection))
            {
                while (moreToRead)
                {
                    using (var session = documentStore.OpenSession())
                    {
                        var tournaments = session.Query<Tournament>().Where(x => !x.IsArchived).Skip(skip).Take(take).ToArray();

                        foreach (var tournament in tournaments)
                        {
                            var tournamentKey = Guid.NewGuid();
                            var createdByUserKey = UsersTable.GetUserKey(connection, site, tournament.CreatedBy.Id);

                            ExecuteInsertCommand(command, tournamentKey, site, tournament, createdByUserKey);
                            TournamentManagersImporter.Import(tournamentKey, connection, site, tournament.Managers);
                            TournamentScorersImporter.Import(tournamentKey, connection, site, tournament.Scorers);
                            CompetitionsImporter.Import(documentStore, connection, tournamentKey, tournament);
                        }

                        skip += tournaments.Length;
                        moreToRead = tournaments.Length > 0 && Program.ReadAll;

                        Log.Progress($"Imported {skip:N0} {site} tournaments of {totalCount:N0}...");
                    }
                }
            }

            CompetitionsImporter.LogStatistics();
            PlayersTable.LogStatistics();

            Log.Statistic($"Maximum tournament name length {_maxNameLength}.");
            Log.Statistic($"Maximum time zone id length {_maxTimeZoneInfoLength}.");
        }

        private static MySqlCommand CreateInsertCommand(MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText =
                "INSERT INTO tournaments (" +
                "TournamentKey, " +
                "Site, " +
                "Name, " +
                "Slug, " +
                "Starts, " +
                "Finishes, " +
                "IsArchived, " +
                "IsMajorTournament, " +
                "SportType, " +
                "TournamentType, " +
                "TimeZoneInfo, " +
                "LastUpdate)" +
                "VALUES (" +
                "@TournamentKey, " +
                "@Site, " +
                "@Name, " +
                "@Slug, " +
                "@Starts, " +
                "@Finishes, " +
                "@IsArchived, " +
                "@IsMajorTournament, " +
                "@SportType, " +
                "@TournamentType, " +
                "@TimeZoneInfo, " +
                "@LastUpdate)";

            command.Parameters.AddWithValue("@TournamentKey", null);
            command.Parameters.AddWithValue("@Site", null);
            command.Parameters.AddWithValue("@Name", null);
            command.Parameters.AddWithValue("@Slug", null);
            command.Parameters.AddWithValue("@Starts", null);
            command.Parameters.AddWithValue("@Finishes", null);
            command.Parameters.AddWithValue("@IsArchived", null);
            command.Parameters.AddWithValue("@IsMajorTournament", null);
            command.Parameters.AddWithValue("@SportType", null);
            command.Parameters.AddWithValue("@TournamentType", null);
            command.Parameters.AddWithValue("@TimeZoneInfo", null);
            command.Parameters.AddWithValue("@LastUpdate", null);

            return command;
        }

        private static void ExecuteInsertCommand(MySqlCommand command, Guid tournamentKey, string site, Tournament tournament, Guid createdByUserKey)
        {
            Log.Debug($"Tournament {tournament.Id}");

            _maxNameLength = Math.Max(_maxNameLength, tournament.Name.Length);
            _maxTimeZoneInfoLength = Math.Max(_maxTimeZoneInfoLength, tournament.TimeZoneInfo.Length);

            if (tournament.Name.Length > 200)
            {
                Log.Error($"Tournament {tournament.Id} name is too long. {tournament.Name}");
                tournament.Name = tournament.Name.Substring(0, 200);
            }

            if (tournament.TimeZoneInfo.Length > 50)
            {
                Log.Error($"Tournament {tournament.Id} TimeZoneInfo is too long. {tournament.TimeZoneInfo}");
                tournament.TimeZoneInfo = tournament.TimeZoneInfo.Substring(0, 50);
            }

            command.Parameters["@TournamentKey"].Value = tournamentKey;
            command.Parameters["@Site"].Value = site;
            command.Parameters["@Name"].Value = tournament.Name;
            command.Parameters["@Slug"].Value = tournament.Slug;
            command.Parameters["@Starts"].Value = tournament.Starts;
            command.Parameters["@Finishes"].Value = tournament.Finishes;
            command.Parameters["@IsArchived"].Value = tournament.IsArchived;
            command.Parameters["@IsMajorTournament"].Value = tournament.IsMajorTournament.GetValueOrDefault();
            command.Parameters["@SportType"].Value = tournament.SportType;
            command.Parameters["@TournamentType"].Value = tournament.TournamentType;
            command.Parameters["@TimeZoneInfo"].Value = tournament.TimeZoneInfo;
            command.Parameters["@LastUpdate"].Value = new DateTime(2024, 1, 1);

            command.ExecuteNonQuery();
        }
    }
}