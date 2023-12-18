using System;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Competitions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class CompetitionBlockImporter
    {
        public static void Import(IDocumentStore documentStore, MySqlConnection connection, Guid competitionKey, Guid tournamentKey, int order, string id)
        {
            {
                var block = GetBlock(documentStore, id);
                var blockProperties = new BlockProperties(block);
                var json = JsonConvert.SerializeObject(blockProperties);

                CompetitionsImporter.ImportCompetition(documentStore, connection, competitionKey, tournamentKey, order, block, "block", json);
            }
        }

        private static Block GetBlock(IDocumentStore documentStore, string id)
        {
            using (var session = documentStore.OpenSession())
            {
                return session.Load<Block>(id);
            }
        }
    }
}