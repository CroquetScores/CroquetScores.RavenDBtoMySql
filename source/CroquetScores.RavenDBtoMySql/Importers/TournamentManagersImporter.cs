using System;
using System.Collections.Generic;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Tables;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class TournamentManagersImporter
    {
        public static void Import(Guid tournamentKey, MySqlConnection connection, string site,
            HashSet<User.Reference> tournamentManagers)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO TournamentManagers (TournamentManagerKey, TournamentKey, UserKey, Created, LastUpdate) " +
                    "VALUES (@TournamentManagerKey, @TournamentKey, @UserKey, @Created, @LastUpdate)";

                command.Parameters.AddWithValue("@TournamentManagerKey", null);
                command.Parameters.AddWithValue("@TournamentKey", null);
                command.Parameters.AddWithValue("@UserKey", null);
                command.Parameters.AddWithValue("@Created", null);
                command.Parameters.AddWithValue("@LastUpdate", null);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var tournamentManager in tournamentManagers)
                {
                    var userKey = UsersTable.TryGetUserKey(connection, site, tournamentManager.Id) ?? UsersImporter.Import(connection, site, tournamentManager);

                    command.Parameters["@TournamentManagerKey"].Value = Guid.NewGuid();
                    command.Parameters["@TournamentKey"].Value = tournamentKey;
                    command.Parameters["@UserKey"].Value = userKey;
                    command.Parameters["@Created"].Value = new DateTime(2024, 1, 1);
                    command.Parameters["@LastUpdate"].Value = new DateTime(2024, 1, 1);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}