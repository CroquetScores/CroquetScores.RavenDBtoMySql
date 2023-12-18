using System;
using System.Collections.Generic;
using System.Linq;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDB.Documents.Types;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;
using Raven.Client;
using TournamentPlayer = CroquetScores.RavenDBtoMySql.TableRows.TournamentPlayer;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class CompetitionsImporter
    {
        private static int _maxNameLength;
        private static int _maxSlugLength;

        public static void Import(IDocumentStore documentStore, MySqlConnection connection, Guid tournamentKey, Tournament tournament)
        {
            var order = 0;

            foreach (var tournamentCompetition in tournament.Competitions)
            {
                order += 1000;

                var id = tournamentCompetition.Id;
                var competitionType = id.Substring(0, id.IndexOf("/", StringComparison.Ordinal));
                var competitionKey = Guid.NewGuid();

                switch (competitionType)
                {
                    case "blocks":
                        CompetitionBlockImporter.Import(documentStore, connection, competitionKey, tournamentKey, order, id);
                        break;

                    case "knockouts":
                        CompetitionKnockoutImporter.Import(documentStore, connection, competitionKey, tournamentKey, order, id);
                        break;

                    case "swisses":
                        CompetitionSwissImporter.Import(documentStore, connection, competitionKey, tournamentKey, order, id);
                        break;

                    case "TeamMatches":
                        CompetitionTeamMatchImporter.Import(documentStore, connection, competitionKey, tournamentKey, order, id);
                        break;

                    default:
                        Log.Error($"competitionType '{competitionType}' is not supported");
                        break;
                }
            }
        }

        private static List<TournamentPlayer> GetTournamentPlayers(MySqlConnection connection, Guid tournamentKey)
        {
            var tournamentPlayers = new List<TournamentPlayer>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM TournamentPlayers " +
                                      "WHERE TournamentKey = @TournamentKey;";
                command.Parameters.AddWithValue("@TournamentKey", tournamentKey);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tournamentPlayers.Add(new TournamentPlayer(reader));
                    }
                }
            }

            return tournamentPlayers;
        }

        public static void ImportCompetition(IDocumentStore documentStore, MySqlConnection connection, Guid competitionKey, Guid tournamentKey, int order, Competition competition, string type, string typeProperties)
        {
            Log.Debug($"Competition {competition.Id}");

            InsertCompetition(connection, competitionKey, tournamentKey, order, competition, type, typeProperties);

            var tournamentPlayerImporters = ImportCompetitionPlayers(connection, competition.Players._Players, tournamentKey, competitionKey);

            ImportGames(connection, tournamentKey, competitionKey, competition.Games, tournamentPlayerImporters);
        }

        private static List<TournamentPlayer> ImportCompetitionPlayers(MySqlConnection connection, List<CompetitionPlayer> competitionPlayers, Guid tournamentKey, Guid competitionKey)
        {
            var tournamentPlayers = GetTournamentPlayers(connection, tournamentKey);

            foreach (var competitionPlayer in competitionPlayers)
            {
                var tournamentPlayer = tournamentPlayers.SingleOrDefault(t => t.RavenDbKey == competitionPlayer._Id);

                if (tournamentPlayer == null)
                {
                    var tournamentPlayerKey = TournamentPlayersImporter.ImportPlayer(connection, tournamentKey, competitionPlayer);

                    tournamentPlayers.Add(new TournamentPlayer(tournamentPlayerKey, tournamentKey, competitionPlayer));
                }
                else
                {
                    ValidateProperty(tournamentKey, competitionKey, nameof(tournamentPlayer.Name), tournamentPlayer.Name, competitionPlayer.Name);
                    ValidateProperty(tournamentKey, competitionKey, nameof(tournamentPlayer.Slug), tournamentPlayer.Slug, competitionPlayer.Slug);

                    if (tournamentPlayer.Representing == null && competitionPlayer.Representing == null)
                    {
                        continue;
                    }

                    if (tournamentPlayer.Representing != null)
                    {
                        if (string.IsNullOrWhiteSpace(competitionPlayer.Representing))
                        {
                            continue;
                        }

                        if (tournamentPlayer.Representing == competitionPlayer.Representing)
                        {
                            continue;
                        }

                        Log.Error($"Tournament player representing '{tournamentPlayer.Representing}' does not match competition player representing '{competitionPlayer.Representing}'");
                    }

                    if (competitionPlayer.Representing != null)
                    {
                        UpdateRepresenting(connection, tournamentPlayer, competitionPlayer.Representing);
                    }
                }
            }

            return tournamentPlayers;
        }

        private static void UpdateRepresenting(MySqlConnection connection, TournamentPlayer tournamentPlayer, string competitionPlayerRepresenting)
        {
            Log.Warning($"Updated representing for {tournamentPlayer.RavenDbKey} {tournamentPlayer.Name} from nothing to {competitionPlayerRepresenting}.");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE TournamentPlayers " +
                                      "SET Representing = @Representing " +
                                      "WHERE TournamentPlayerKey = @TournamentPlayerKey;";

                command.Parameters.AddWithValue("@Representing", competitionPlayerRepresenting);
                command.Parameters.AddWithValue("@TournamentPlayerKey", tournamentPlayer.TournamentPlayerKey);
                command.ExecuteNonQuery();
            }
        }

        private static void ValidateProperty(Guid tournamentKey, Guid competitionKey, string propertyName, string tournamentPropertyValue, string competitionPropertyValue)
        {
            if (string.Equals(tournamentPropertyValue.Trim(), competitionPropertyValue.Trim(), StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            Log.Error($"TournamentPlayer.{propertyName} '{tournamentPropertyValue}' != CompetitionPlayer.{propertyName} '{competitionPropertyValue}'. tournamentKey = {tournamentKey} competitionKey {competitionKey}");
        }

        private static void InsertCompetition(MySqlConnection connection, Guid competitionKey, Guid tournamentKey, int order, Competition competition, string type, string typeProperties)
        {
            ValidateColumnLengths(competition);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Competitions (" +
                                      "CompetitionKey," +
                                      "TournamentKey," +
                                      "OrderBy," +
                                      "Name," +
                                      "Slug," +
                                      "Type," +
                                      "TypeProperties," +
                                      "IsArchived," +
                                      "RavenDbKey," +
                                      "Created," +
                                      "LastUpdate) " +
                                      "VALUES (" +
                                      "@CompetitionKey," +
                                      "@TournamentKey," +
                                      "@OrderBy," +
                                      "@Name," +
                                      "@Slug," +
                                      "@Type," +
                                      "@TypeProperties," +
                                      "@IsArchived," +
                                      "@RavenDbKey," +
                                      "@Created," +
                                      "@LastUpdate);";

                command.Parameters.AddWithValue("@CompetitionKey", competitionKey);
                command.Parameters.AddWithValue("@TournamentKey", tournamentKey);
                command.Parameters.AddWithValue("@OrderBy", order);
                command.Parameters.AddWithValue("@Name", competition.Name);
                command.Parameters.AddWithValue("@Slug", competition.Slug);
                command.Parameters.AddWithValue("@Type", type);
                command.Parameters.AddWithValue("@TypeProperties", typeProperties);
                command.Parameters.AddWithValue("@IsArchived", competition.IsArchived);
                command.Parameters.AddWithValue("@RavenDbKey", competition.Id);
                command.Parameters.AddWithValue("@Created", new DateTime(2024, 1, 1));
                command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));

                command.ExecuteNonQuery();
            }
        }

        private static void ValidateColumnLengths(Competition competition)
        {
            _maxNameLength = Math.Max(_maxNameLength, competition.Name.Length);
            _maxSlugLength = Math.Max(_maxSlugLength, competition.Slug.Length);

            if (competition.Name.Length > 100)
            {
                Log.Error($"Competition {competition.Id} name is too long. {competition.Name}");
                competition.Name = competition.Name.Substring(0, 100);
            }

            if (competition.Slug.Length <= 100)
            {
                return;
            }

            Log.Error($"Competition {competition.Id} slug is too long. {competition.Slug}");
            competition.Slug = competition.Slug.Substring(0, 100);
        }

        public static void ImportGames(MySqlConnection connection, Guid tournamentKey, Guid competitionKey, CompetitionGames games, List<TournamentPlayer> tournamentPlayerImporters)
        {
            var order = 0;

            using (var command = CreateInsertGameCommand(connection))
            {
                foreach (var game in games._Games)
                {
                    order += 1000;

                    command.Parameters["@GameKey"].Value = Guid.NewGuid();
                    command.Parameters["@CompetitionKey"].Value = competitionKey;
                    command.Parameters["@OrderBy"].Value = order;
                    command.Parameters["@WinnerPlayerKey"].Value = GetTournamentPlayerKey(game.Winner.PlayerId, tournamentPlayerImporters);
                    command.Parameters["@WinnerScore"].Value = game.Winner.Score;
                    command.Parameters["@LoserPlayerKey"].Value = GetTournamentPlayerKey(game.Loser.PlayerId, tournamentPlayerImporters);
                    command.Parameters["@LoserScore"].Value = game.Loser.Score;
                    command.Parameters["@Created"].Value = game.CreatedAt;
                    command.Parameters["@LastUpdate"].Value = new DateTime(2024, 1, 1);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static Guid GetTournamentPlayerKey(int tournamentPlayerId, IEnumerable<TournamentPlayer> tournamentPlayerImporters)
        {
            return tournamentPlayerImporters.Single(x => x.RavenDbKey == tournamentPlayerId).TournamentPlayerKey;
        }

        private static MySqlCommand CreateInsertGameCommand(MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO Games (" +
                                  "GameKey," +
                                  "CompetitionKey," +
                                  "OrderBy," +
                                  "WinnerPlayerKey," +
                                  "WinnerScore," +
                                  "LoserPlayerKey," +
                                  "LoserScore," +
                                  "Created," +
                                  "LastUpdate) " +
                                  "VALUES (" +
                                  "@GameKey," +
                                  "@CompetitionKey," +
                                  "@OrderBy," +
                                  "@WinnerPlayerKey," +
                                  "@WinnerScore," +
                                  "@LoserPlayerKey," +
                                  "@LoserScore," +
                                  "@Created," +
                                  "@LastUpdate);";

            command.Parameters.AddWithValue("@GameKey", null);
            command.Parameters.AddWithValue("@CompetitionKey", null);
            command.Parameters.AddWithValue("@OrderBy", null);
            command.Parameters.AddWithValue("@WinnerPlayerKey", null);
            command.Parameters.AddWithValue("@WinnerScore", null);
            command.Parameters.AddWithValue("@LoserPlayerKey", null);
            command.Parameters.AddWithValue("@LoserScore", null);
            command.Parameters.AddWithValue("@Created", null);
            command.Parameters.AddWithValue("@LastUpdate", null);

            return command;
        }

        public static void LogStatistics()
        {
            Log.Statistic($"Longest Competition.Name {_maxNameLength}");
            Log.Statistic($"Longest Competition.Slug {_maxSlugLength}");
        }
    }
}