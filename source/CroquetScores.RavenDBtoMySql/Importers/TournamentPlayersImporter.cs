using System;
using System.Linq;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDB.Documents.Types;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class TournamentPlayersImporter
    {
        private static int _maxNameLength;
        private static int _maxRepresentingLength;
        private static int _maxSlugLength;

        public static void Import(IDocumentStore documentStore, MySqlConnection connection, Guid tournamentKey, Tournament tournament)
        {
            using (var session = documentStore.OpenSession())
            {
                var multipleTournamentPlayers = session
                    .Query<TournamentPlayers>("TournamentsPlayers/ByIsArchivedAndTournamentId")
                    .Where(x => x.Tournament.Id == tournament.Id)
                    .ToArray();

                if (multipleTournamentPlayers.Length == 0)
                {
                    return;
                }

                if (multipleTournamentPlayers.Length > 1)
                {
                    Log.Error($"There are {multipleTournamentPlayers.Length} TournamentPlayers for {tournament.Id}.");
                }

                var tournamentPlayers = multipleTournamentPlayers[0];

                using (var command = CreateInsertCommand(connection, tournamentKey))
                {
                    foreach (var tournamentPlayer in tournamentPlayers._Players)
                    {
                        ImportPlayer(command, tournamentKey, tournamentPlayer);
                    }
                }
            }
        }

        public static Guid ImportPlayer(MySqlConnection connection, Guid tournamentKey, TournamentPlayer tournamentPlayer)
        {
            using (var command = CreateInsertCommand(connection, tournamentKey))
            {
                return ImportPlayer(command, tournamentKey, tournamentPlayer);
            }
        }

        public static Guid ImportPlayer(MySqlCommand command, Guid tournamentKey, TournamentPlayer tournamentPlayer)
        {
            ValidateColumnLengths(tournamentPlayer);

            var tournamentPlayerKey = Guid.NewGuid();

            command.Parameters["@TournamentPlayerKey"].Value = tournamentPlayerKey;
            command.Parameters["@TournamentKey"].Value = tournamentKey;
            command.Parameters["@Name"].Value = tournamentPlayer.Name;
            command.Parameters["@Representing"].Value = tournamentPlayer.Representing;
            command.Parameters["@Slug"].Value = tournamentPlayer.Slug;
            command.Parameters["@RavenDbKey"].Value = tournamentPlayer._Id;

            command.ExecuteNonQuery();

            return tournamentPlayerKey;
        }

        private static MySqlCommand CreateInsertCommand(MySqlConnection connection, Guid tournamentKey)
        {
            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO TournamentPlayers (" +
                                  "TournamentPlayerKey," +
                                  "TournamentKey," +
                                  "Name," +
                                  "Representing," +
                                  "Slug," +
                                  "Created," +
                                  "LastUpdate," +
                                  "RavenDbKey) " +
                                  "VALUES (" +
                                  "@TournamentPlayerKey," +
                                  "@TournamentKey," +
                                  "@Name," +
                                  "@Representing," +
                                  "@Slug," +
                                  "@Created," +
                                  "@LastUpdate," +
                                  "@RavenDbKey)";


            command.Parameters.AddWithValue("@TournamentPlayerKey", null);
            command.Parameters.AddWithValue("@TournamentKey", tournamentKey);
            command.Parameters.AddWithValue("@Name", null);
            command.Parameters.AddWithValue("@Representing", null);
            command.Parameters.AddWithValue("@Slug", null);
            command.Parameters.AddWithValue("@Created", new DateTime(2024, 1, 1));
            command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));
            command.Parameters.AddWithValue("@RavenDbKey", null);

            return command;
        }

        private static void ValidateColumnLengths(TournamentPlayer tournamentPlayer)
        {
            _maxNameLength = Math.Max(_maxNameLength, tournamentPlayer.Name.Length);
            _maxRepresentingLength = Math.Max(_maxRepresentingLength, tournamentPlayer.Representing.GetLength());
            _maxSlugLength = Math.Max(_maxSlugLength, tournamentPlayer.Slug.Length);

            if (tournamentPlayer.Name.Length > 500)
            {
                Log.Error($"Tournament Player {tournamentPlayer._Id}/{tournamentPlayer.Name}/{tournamentPlayer.Representing} name is too long.");
                tournamentPlayer.Name = tournamentPlayer.Name.Substring(0, 500);
            }

            if (tournamentPlayer.Representing.GetLength() > 100)
            {
                Log.Error($"Tournament Player {tournamentPlayer._Id}/{tournamentPlayer.Name}/{tournamentPlayer.Representing} representing is too long.");
                tournamentPlayer.Representing = tournamentPlayer.Representing.Substring(0, 100);
            }

            if (tournamentPlayer.Slug.Length <= 500)
            {
                return;
            }

            Log.Error($"Tournament Player {tournamentPlayer._Id}/{tournamentPlayer.Name}/{tournamentPlayer.Representing} slug is too long.");
            tournamentPlayer.Slug = tournamentPlayer.Slug.Substring(0, 500);
        }

        public static void LogStatistics()
        {
            Log.Statistic($"Longest TournamentPlayer.Name {_maxNameLength}");
            Log.Statistic($"Longest TournamentPlayer.Representing {_maxRepresentingLength}");
            Log.Statistic($"Longest TournamentPlayer.Slug {_maxSlugLength}");
        }
    }
}