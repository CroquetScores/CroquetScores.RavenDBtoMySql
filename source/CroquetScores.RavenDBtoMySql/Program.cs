﻿using Raven.Client.Document;
using Raven.Client;
using System;
using System.Configuration;
using System.Linq;
using System.Web;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDBtoMySql.Support;

namespace CroquetScores.RavenDBtoMySql
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                Console.WriteLine("Opening document store...");
                using (var documentStore = RavenDbSupport.InitializeDocumentStore())
                {
                    Console.WriteLine("Creating the MySQL database...");
                    MySqlSupport.CreateDatabase();
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
