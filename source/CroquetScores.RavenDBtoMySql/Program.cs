using System;
using CroquetScores.RavenDBtoMySql.Importers;
using CroquetScores.RavenDBtoMySql.Support;

namespace CroquetScores.RavenDBtoMySql
{
    internal class Program
    {
        public const bool ReadAll = true;

        private static void Main()
        {
            try
            {
                Log.Start();
                Log.Progress("Creating the database...");
                MySqlDatabase.CreateDatabase();

                Log.Progress("Opening the database...");
                using (var connection = MySqlDatabase.OpenDatabase())
                {
                    var sites = new[] { "croquetscores.com", "gateballscores.com" };

                    foreach (var site in sites)
                    {
                        Log.Progress($"Opening document store for {site}...");
                        using (var documentStore = RavenDbDatabase.InitializeDocumentStore(site))
                        {
                            {
                                UsersImporter.Import(documentStore, connection, site);
                                TournamentsImporter.Import(documentStore, connection, site);
                            }
                        }
                    }
                }

                Log.Progress("Success!");
                Log.Finish();
                Console.WriteLine();
                Console.WriteLine($"Info logged to '{Log.FileName}'.");
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