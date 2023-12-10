using Raven.Client.Document;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CroquetScores.RavenDBtoMySql.Support
{
    internal class RavenDbSupport
    {
        public static IDocumentStore InitializeDocumentStore()
        {
            var ravenDbConnectionString = GetRavenDbConnectionString();
            var values = HttpUtility.ParseQueryString(ConvertConnectionStringToQueryString(ravenDbConnectionString));
            var documentStore = new DocumentStore()
            {
                ApiKey = values["ApiKey"],
                Url = values["Url"],
                DefaultDatabase = values["Database"]
            };

            documentStore.Initialize();
            return documentStore;
        }

        private static string GetRavenDbConnectionString()
        {
            var ravenDbConnectionString = ConfigurationManager.AppSettings["RavenDbConnectionString"];

            if (string.IsNullOrWhiteSpace(ravenDbConnectionString))
            {
                throw new Exception("RavenDbConnectionString has not been set in AppSettings.config.");
            }

            ravenDbConnectionString = ConvertConnectionStringToQueryString(ravenDbConnectionString);

            return ravenDbConnectionString;
        }

        private static string ConvertConnectionStringToQueryString(string connectionString)
        {
            return connectionString.Trim(System.Convert.ToChar(";")).Replace(";", "&");
        }
    }
}
