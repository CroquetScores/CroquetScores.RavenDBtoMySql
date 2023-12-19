using System;
using CroquetScores.RavenDB.Documents.Types;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class CompetitionPlayersImporter
    {
        private static int _maxNameLength;
        private static int _maxRepresentingLength;
        private static int _maxSlugLength;

        public static Guid ImportPlayer(MySqlConnection connection, Guid competitionKey, int orderBy, CompetitionPlayer competitionPlayer)
        {
            using (var command = CreateInsertCommand(connection, competitionKey))
            {
                return ImportPlayer(command, competitionKey, orderBy, competitionPlayer);
            }
        }

        public static Guid ImportPlayer(MySqlCommand command, Guid competitionKey, int orderBy, CompetitionPlayer competitionPlayer)
        {
            ValidateColumnLengths(competitionPlayer);

            var competitionPlayerKey = Guid.NewGuid();

            command.Parameters["@CompetitonPlayerKey"].Value = competitionPlayerKey;
            command.Parameters["@CompetitonKey"].Value = competitionKey;
            command.Parameters["@OrderBy"].Value = orderBy;
            command.Parameters["@Name"].Value = competitionPlayer.Name;
            command.Parameters["@Representing"].Value = competitionPlayer.Representing;
            command.Parameters["@Slug"].Value = competitionPlayer.Slug;
            command.Parameters["@RavenDbKey"].Value = competitionPlayer._Id;

            command.ExecuteNonQuery();

            return competitionPlayerKey;
        }

        private static MySqlCommand CreateInsertCommand(MySqlConnection connection, Guid competitionKey)
        {
            var command = connection.CreateCommand();

            command.CommandText = "INSERT INTO CompetitonPlayers (" +
                                  "CompetitonPlayerKey," +
                                  "CompetitonKey," +
                                  "OrderBy," +
                                  "Name," +
                                  "Representing," +
                                  "Slug," +
                                  "Created," +
                                  "LastUpdate," +
                                  "RavenDbKey) " +
                                  "VALUES (" +
                                  "@CompetitionPlayerKey," +
                                  "@CompetitionKey," +
                                  "@OrderBy," +
                                  "@Name," +
                                  "@Representing," +
                                  "@Slug," +
                                  "@Created," +
                                  "@LastUpdate," +
                                  "@RavenDbKey)";


            command.Parameters.AddWithValue("@CompetitionPlayerKey", null);
            command.Parameters.AddWithValue("@CompetitionKey", competitionKey);
            command.Parameters.AddWithValue("@OrderBy", null);
            command.Parameters.AddWithValue("@Name", null);
            command.Parameters.AddWithValue("@Representing", null);
            command.Parameters.AddWithValue("@Slug", null);
            command.Parameters.AddWithValue("@Created", new DateTime(2024, 1, 1));
            command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));
            command.Parameters.AddWithValue("@RavenDbKey", null);

            return command;
        }

        private static void ValidateColumnLengths(CompetitionPlayer competitionPlayer)
        {
            _maxNameLength = Math.Max(_maxNameLength, competitionPlayer.Name.Length);
            _maxRepresentingLength = Math.Max(_maxRepresentingLength, competitionPlayer.Representing.GetLength());
            _maxSlugLength = Math.Max(_maxSlugLength, competitionPlayer.Slug.Length);

            if (competitionPlayer.Name.Length > 500)
            {
                Log.Error($"Competition Player {competitionPlayer._Id}/{competitionPlayer.Name}/{competitionPlayer.Representing} name is too long.");
                competitionPlayer.Name = competitionPlayer.Name.Substring(0, 500);
            }

            if (competitionPlayer.Representing.GetLength() > 100)
            {
                Log.Error($"Competition Player {competitionPlayer._Id}/{competitionPlayer.Name}/{competitionPlayer.Representing} representing is too long.");
                competitionPlayer.Representing = competitionPlayer.Representing.Substring(0, 100);
            }

            if (competitionPlayer.Slug.Length <= 500)
            {
                return;
            }

            Log.Error($"Competition Player {competitionPlayer._Id}/{competitionPlayer.Name}/{competitionPlayer.Representing} slug is too long.");
            competitionPlayer.Slug = competitionPlayer.Slug.Substring(0, 500);
        }

        public static void LogStatistics()
        {
            Log.Statistic($"Longest competitionPlayer.Name {_maxNameLength}");
            Log.Statistic($"Longest competitionPlayer.Representing {_maxRepresentingLength}");
            Log.Statistic($"Longest competitionPlayer.Slug {_maxSlugLength}");
        }
    }
}