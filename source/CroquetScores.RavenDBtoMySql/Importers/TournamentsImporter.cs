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
        private static int _maxTimeZoneIdLength;

        public static void Import(IDocumentStore documentStore, MySqlConnection connection, string site)
        {
            Log.Progress("Importing Tournaments data...");

            int totalCount;
            var skip = 0;
            const int take = 100;
            var moreToRead = true;

            using (var session = documentStore.OpenSession())
            {
                totalCount = session.Query<Tournament>().Count();
            }

            Log.Statistic($"{totalCount:N0} {site} tournaments to import...");

            using (var command = CreateInsertCommand(connection))
            {
                while (moreToRead)
                {
                    using (var session = documentStore.OpenSession())
                    {
                        var tournaments = session.Query<Tournament>().Skip(skip).Take(take).ToArray();

                        foreach (var tournament in tournaments)
                        {
                            var tournamentKey = Guid.NewGuid();
                            var createdByUserKey = UsersTable.GetUserKey(connection, site, tournament.CreatedBy.Id);

                            ExecuteInsertCommand(command, tournamentKey, site, tournament, createdByUserKey);
                            TournamentManagersImporter.Import(tournamentKey, connection, site, tournament.Managers);
                            TournamentScorersImporter.Import(tournamentKey, connection, site, tournament.Scorers);
                        }

                        skip += tournaments.Length;
                        moreToRead = tournaments.Length > 0;

                        Log.Progress($"Imported {skip:N0} {site} tournaments of {totalCount:N0}...");
                    }
                }
            }

            Log.Statistic($"Maximum tournament name length {_maxNameLength}.");
            Log.Statistic($"Maximum time zone id length {_maxTimeZoneIdLength}.");
        }

        private static MySqlCommand CreateInsertCommand(MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText =
                "INSERT INTO Tournaments (" +
                "TournamentKey, " +
                "Site, " +
                "Name, " +
                "Slug, " +
                "Start, " +
                "Finish, " +
                "CreateBy_UserKey, " +
                "IsArchived, " +
                "IsMajorTournament, " +
                "SportType, " +
                "TournamentType, " +
                "TimeZoneId, " +
                "Created, " +
                "LastUpdate)" +
                "VALUES (" +
                "@TournamentKey, " +
                "@Site, " +
                "@Name, " +
                "@Slug, " +
                "@Start, " +
                "@Finish, " +
                "@CreateBy_UserKey, " +
                "@IsArchived, " +
                "@IsMajorTournament, " +
                "@SportType, " +
                "@TournamentType, " +
                "@TimeZoneId, " +
                "@Created, " +
                "@LastUpdate)";

            command.Parameters.AddWithValue("@TournamentKey", null);
            command.Parameters.AddWithValue("@Site", null);
            command.Parameters.AddWithValue("@Name", null);
            command.Parameters.AddWithValue("@Slug", null);
            command.Parameters.AddWithValue("@Start", null);
            command.Parameters.AddWithValue("@Finish", null);
            command.Parameters.AddWithValue("@CreateBy_UserKey", null);
            command.Parameters.AddWithValue("@IsArchived", null);
            command.Parameters.AddWithValue("@IsMajorTournament", null);
            command.Parameters.AddWithValue("@SportType", null);
            command.Parameters.AddWithValue("@TournamentType", null);
            command.Parameters.AddWithValue("@TimeZoneId", null);
            command.Parameters.AddWithValue("@Created", null);
            command.Parameters.AddWithValue("@LastUpdate", null);

            return command;
        }

        private static void ExecuteInsertCommand(MySqlCommand command, Guid tournamentKey, string site,
            Tournament tournament, Guid createdByUserKey)
        {
            _maxNameLength = Math.Max(_maxNameLength, tournament.Name.Length);
            _maxTimeZoneIdLength = Math.Max(_maxTimeZoneIdLength, tournament.TimeZoneId.Length);

            if (_maxNameLength > 600)
            {
                throw new Exception($"Tournament {tournament.Id} name is too long. {tournament.Name}");
            }

            if (_maxTimeZoneIdLength > 50)
            {
                throw new Exception($"Tournament {tournament.Id} TimeZoneId is too long. {tournament.TimeZoneId}");
            }

            command.Parameters["@TournamentKey"].Value = tournamentKey;
            command.Parameters["@Site"].Value = site;
            command.Parameters["@Name"].Value = tournament.Name;
            command.Parameters["@Slug"].Value = tournament.Slug;
            command.Parameters["@Start"].Value = tournament.Start;
            command.Parameters["@Finish"].Value = tournament.Finish;
            command.Parameters["@CreateBy_UserKey"].Value = createdByUserKey;
            command.Parameters["@IsArchived"].Value = tournament.IsArchived;
            command.Parameters["@IsMajorTournament"].Value = tournament.IsMajorTournament.GetValueOrDefault();
            command.Parameters["@SportType"].Value = tournament.SportType;
            command.Parameters["@TournamentType"].Value = tournament.TournamentType;
            command.Parameters["@TimeZoneId"].Value = tournament.TimeZoneId;
            command.Parameters["@Created"].Value = new DateTime(2024, 1, 1);
            command.Parameters["@LastUpdate"].Value = new DateTime(2024, 1, 1);

            command.ExecuteNonQuery();
        }
    }
}