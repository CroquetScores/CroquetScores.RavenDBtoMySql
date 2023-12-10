using Raven.Client.Document;
using Raven.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using CroquetScores.RavenDB.Documents;

namespace CroquetScores.RavenDBtoMySql
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                Console.WriteLine("Opening document store...");
                using (var documentStore = GetDocumentStore())
                {
                    using (var session = documentStore.OpenSession())
                    {
                        var users = session.Query<User>().ToArray();

                        Console.WriteLine($"Got {users.Length} users.");
                    }
                }
            }
            catch (Exception exception)
            {
                var foregroundColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Red;

                Console.Error.WriteLine();
                Console.Error.WriteLine("The following error occurred:");
                Console.Error.WriteLine();
                Console.Error.WriteLine(exception);
                Console.Error.WriteLine();

                Console.ForegroundColor = foregroundColor;

                Environment.Exit(1);
            }

        }
        private static IDocumentStore GetDocumentStore()
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
