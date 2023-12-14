﻿using System;
using System.Collections.Generic;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Tables;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class TournamentScorersImporter
    {
        public static void Import(Guid tournamentKey, MySqlConnection connection, string site,
            HashSet<User.Reference> tournamentScorers)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO TournamentScorers (TournamentScorerKey, TournamentKey, UserKey, Created, LastUpdate) " +
                    "VALUES (@TournamentScorerKey, @TournamentKey, @UserKey, @Created, @LastUpdate)";

                command.Parameters.AddWithValue("@TournamentScorerKey", null);
                command.Parameters.AddWithValue("@TournamentKey", null);
                command.Parameters.AddWithValue("@UserKey", null);
                command.Parameters.AddWithValue("@Created", null);
                command.Parameters.AddWithValue("@LastUpdate", null);

                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var tournamentScorer in tournamentScorers)
                {
                    var userKey = UsersTable.TryGetUserKey(connection, site, tournamentScorer.Id);

                    if (userKey == null)
                    {
                        userKey = UsersImporter.Import(connection, site, tournamentScorer);
                    }

                    command.Parameters["@TournamentScorerKey"].Value = Guid.NewGuid();
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