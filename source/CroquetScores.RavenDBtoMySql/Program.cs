using System;
using CroquetScores.RavenDBtoMySql.Importers;
using CroquetScores.RavenDBtoMySql.Support;

namespace CroquetScores.RavenDBtoMySql
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                Console.WriteLine("Creating the database...");
                MySqlDatabase.CreateDatabase();

                Console.WriteLine("Opening the database...");
                using (var connection = MySqlDatabase.OpenDatabase())
                {
                    var sites = new[] { "croquetscores.com", "gateballscores.com" };

                    foreach (var site in sites)
                    {
                        Console.WriteLine($"Opening document store for {site}...");
                        using (var documentStore = RavenDbDatabase.InitializeDocumentStore(site))
                        {
                            {
                                UsersImporter.Import(documentStore, connection, site);
                                TournamentsImporter.Import(documentStore, connection, site);
                            }
                        }
                    }
                }

                Console.WriteLine("Success!");
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
    }
}