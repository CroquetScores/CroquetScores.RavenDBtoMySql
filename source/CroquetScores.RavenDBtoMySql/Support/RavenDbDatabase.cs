using System;
using System.Configuration;
using System.Web;
using Raven.Client;
using Raven.Client.Document;

namespace CroquetScores.RavenDBtoMySql.Support
{
    internal class RavenDbDatabase
    {
        public static IDocumentStore InitializeDocumentStore(string site)
        {
            var ravenDbConnectionString = GetRavenDbConnectionString(site);
            var values = HttpUtility.ParseQueryString(ConvertConnectionStringToQueryString(ravenDbConnectionString));
            var documentStore = new DocumentStore
            {
                ApiKey = values["ApiKey"],
                Url = values["Url"],
                DefaultDatabase = values["Database"]
            };

            documentStore.Initialize();
            return documentStore;
        }

        private static string GetRavenDbConnectionString(string site)
        {
            var ravenDbConnectionString = ConfigurationManager.AppSettings[$"RavenDb:{site}"];

            if (string.IsNullOrWhiteSpace(ravenDbConnectionString))
                throw new Exception($"RavenDb:{site} has not been set in AppSettings.config.");

            ravenDbConnectionString = ConvertConnectionStringToQueryString(ravenDbConnectionString);

            return ravenDbConnectionString;
        }

        private static string ConvertConnectionStringToQueryString(string connectionString)
        {
            return connectionString.Trim(Convert.ToChar(";")).Replace(";", "&");
        }
    }
}