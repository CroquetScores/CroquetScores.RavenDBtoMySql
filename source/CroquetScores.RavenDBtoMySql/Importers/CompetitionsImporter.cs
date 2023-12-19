using System;
using System.Collections.Generic;
using System.Linq;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDB.Documents.Types;
using CroquetScores.RavenDBtoMySql.Support;
using CroquetScores.RavenDBtoMySql.TableRows;
using CroquetScores.RavenDBtoMySql.Tables;
using MySql.Data.MySqlClient;
using Raven.Client;

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

        public static void ImportCompetition(IDocumentStore documentStore, MySqlConnection connection, Guid competitionKey, Guid tournamentKey, int order, Competition competition, string type, string typeProperties)
        {
            Log.Debug($"Competition {competition.Id}");

            InsertCompetition(connection, competitionKey, tournamentKey, order, competition, type, typeProperties);

            var tournamentPlayerImporters = ImportCompetitionPlayers(connection, competition.Players._Players, tournamentKey, competitionKey);

            ImportGames(connection, tournamentKey, competitionKey, competition.Games, tournamentPlayerImporters);
        }

        private static List<CompetitionPlayerRow> ImportCompetitionPlayers(MySqlConnection connection, List<CompetitionPlayer> ravenDbCompetitionPlayers, Guid tournamentKey, Guid competitionKey)
        {
            var competitionPlayerRows = new List<CompetitionPlayerRow>();
            var orderBy = 0;

            using (var insertCompetitionPlayerCommand = CreateInsertCompetitionPlayerCommand(connection))
            {
                foreach (var ravenDbCompetitionPlayer in ravenDbCompetitionPlayers)
                {
                    orderBy += 1000;
                    var playerKey = PlayersTable.FindOrAddByName(connection, ravenDbCompetitionPlayer.Name);
                    var competitionPlayerRow = new CompetitionPlayerRow(Guid.NewGuid(), competitionKey, playerKey, orderBy, ravenDbCompetitionPlayer);

                    InsertCompetitionPlayerRow(insertCompetitionPlayerCommand, competitionPlayerRow);

                    competitionPlayerRows.Add(competitionPlayerRow);
                }
            }

            return competitionPlayerRows;
        }

        private static void InsertCompetition(MySqlConnection connection, Guid competitionKey, Guid tournamentKey, int order, Competition competition, string type, string typeProperties)
        {
            ValidateColumnLengths(competition);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO competitions (" +
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

        private static void InsertCompetitionPlayerRow(MySqlCommand command, CompetitionPlayerRow competitionPlayerRow)
        {
            command.Parameters["@CompetitionPlayerKey"].Value = competitionPlayerRow.CompetitionPlayerKey;
            command.Parameters["@CompetitionKey"].Value = competitionPlayerRow.CompetitionKey;
            command.Parameters["@PlayerKey"].Value = competitionPlayerRow.PlayerKey;
            command.Parameters["@OrderBy"].Value = competitionPlayerRow.OrderBy;
            command.Parameters["@Representing"].Value = competitionPlayerRow.Representing;
            command.Parameters["@Created"].Value = competitionPlayerRow.Created;
            command.Parameters["@LastUpdate"].Value = competitionPlayerRow.LastUpdated;
            command.Parameters["@RavenDbKey"].Value = competitionPlayerRow.RavenDbKey;

            command.ExecuteNonQuery();
        }

        private static MySqlCommand CreateInsertCompetitionPlayerCommand(MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO competition_players (" +
                                  "CompetitionPlayerKey," +
                                  "CompetitionKey," +
                                  "PlayerKey," +
                                  "OrderBy," +
                                  "Representing," +
                                  "Created," +
                                  "LastUpdate," +
                                  "RavenDbKey) " +
                                  "VALUES (" +
                                  "@CompetitionPlayerKey," +
                                  "@CompetitionKey," +
                                  "@PlayerKey," +
                                  "@OrderBy," +
                                  "@Representing," +
                                  "@Created," +
                                  "@LastUpdate," +
                                  "@RavenDbKey);";

            command.Parameters.AddWithValue("@CompetitionPlayerKey", null);
            command.Parameters.AddWithValue("@CompetitionKey", null);
            command.Parameters.AddWithValue("@PlayerKey", null);
            command.Parameters.AddWithValue("@OrderBy", null);
            command.Parameters.AddWithValue("@Representing", null);
            command.Parameters.AddWithValue("@Created", null);
            command.Parameters.AddWithValue("@LastUpdate", null);
            command.Parameters.AddWithValue("@RavenDbKey", null);

            return command;
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

        public static void ImportGames(MySqlConnection connection, Guid tournamentKey, Guid competitionKey, CompetitionGames games, List<CompetitionPlayerRow> competitionPlayerRows)
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
                    command.Parameters["@WinnerPlayerKey"].Value = GetTournamentPlayerKey(game.Winner.PlayerId, competitionPlayerRows);
                    command.Parameters["@WinnerScore"].Value = game.Winner.Score;
                    command.Parameters["@LoserPlayerKey"].Value = GetTournamentPlayerKey(game.Loser.PlayerId, competitionPlayerRows);
                    command.Parameters["@LoserScore"].Value = game.Loser.Score;
                    command.Parameters["@Created"].Value = game.CreatedAt;
                    command.Parameters["@LastUpdate"].Value = new DateTime(2024, 1, 1);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static Guid GetTournamentPlayerKey(int tournamentPlayerId, IEnumerable<CompetitionPlayerRow> tournamentPlayerImporters)
        {
            return tournamentPlayerImporters.Single(x => x.RavenDbKey == tournamentPlayerId).PlayerKey;
        }

        private static MySqlCommand CreateInsertGameCommand(MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO games (" +
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