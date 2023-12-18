using System;
using System.Collections.Generic;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Competitions;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class CompetitionSwissImporter
    {
        public static void Import(IDocumentStore documentStore, MySqlConnection connection, Guid tournamentKey, Guid competitionKey, string id)
        {
            {
                var swiss = GetRavenDbSwiss(documentStore, id);
                var swissProperties = new SwissProperties(GetCarryForwardCompetitions(connection, swiss));
                var json = JsonConvert.SerializeObject(swissProperties);

                CompetitionsImporter.ImportCompetition(documentStore, connection, tournamentKey, competitionKey, swiss, "swiss", json);
            }
        }

        private static Guid[] GetCarryForwardCompetitions(MySqlConnection connection, Swiss swiss)
        {
            if (swiss.CarryForwardCompetitions == null)
            {
                return new Guid[] {};
            }

            var carryForwardCompetitions = new List<Guid>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT CompetitionKey FROM Competitions " +
                                      "WHERE RavenDbKey = @RavenDbKey;";

                command.Parameters.AddWithValue("@RavenDbKey", null);

                foreach (var carryForwardCompetition in swiss.CarryForwardCompetitions)
                {
                    command.Parameters["@RavenDbKey"].Value = carryForwardCompetition.Id;

                    var competitionKey = command.ExecuteScalar();

                    if (competitionKey == null)
                    {
                        Log.Warning($"Could not find Carry Forward Competition '{carryForwardCompetition.Id}");
                        continue;
                    }
                    carryForwardCompetitions.Add(Guid.Parse(competitionKey.ToString()));
                }
            }

            return carryForwardCompetitions.ToArray();
        }

        private static Swiss GetRavenDbSwiss(IDocumentStore documentStore, string id)
        {
            using (var session = documentStore.OpenSession())
            {
                return session.Load<Swiss>(id);
            }
        }
    }
}