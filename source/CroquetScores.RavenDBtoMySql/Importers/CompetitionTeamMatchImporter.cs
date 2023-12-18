using System;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Competitions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class CompetitionTeamMatchImporter
    {
        public static void Import(IDocumentStore documentStore, MySqlConnection connection, Guid tournamentKey, Guid competitionKey, string id)
        {
            {
                var teamMatch = GetTeamMatch(documentStore, id);
                var teamMatchProperties = new TeamMatchProperties(teamMatch);
                var json = JsonConvert.SerializeObject(teamMatchProperties);

                CompetitionsImporter.ImportCompetition(documentStore, connection, tournamentKey, competitionKey, teamMatch, "teamMatch", json);
            }
        }

        private static TeamMatch GetTeamMatch(IDocumentStore documentStore, string id)
        {
            using (var session = documentStore.OpenSession())
            {
                return session.Load<TeamMatch>(id);
            }
        }
    }
}