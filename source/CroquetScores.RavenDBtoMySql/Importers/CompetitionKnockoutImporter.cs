using System;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Competitions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class CompetitionKnockoutImporter
    {
        public static void Import(IDocumentStore documentStore, MySqlConnection connection, Guid tournamentKey, Guid competitionKey, string id)
        {
            {
                var knockout = GetKnockout(documentStore, id);
                var knockoutProperties = new KnockoutProperties(knockout);
                var json = JsonConvert.SerializeObject(knockoutProperties);

                CompetitionsImporter.ImportCompetition(documentStore, connection, tournamentKey, competitionKey, knockout, "knockout", json);
            }
        }

        private static Knockout GetKnockout(IDocumentStore documentStore, string id)
        {
            using (var session = documentStore.OpenSession())
            {
                return session.Load<Knockout>(id);
            }
        }
    }
}