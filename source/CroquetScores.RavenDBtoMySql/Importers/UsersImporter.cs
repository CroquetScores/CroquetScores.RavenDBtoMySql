using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using CroquetScores.RavenDB.Documents;
using MySql.Data.MySqlClient;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class UsersImporter
    {
        private static int _maxNameLength;
        private static int _maxEmailAddressLength;
        private static int _maxSlugLength;

        public static void Import(IDocumentStore documentStore, MySqlConnection connection, string site)
        {
            Console.WriteLine("Importing users data...");

            int totalCount;
            var skip = 0;
            const int take = 1024;
            var moreToRead = true;

            using (var session = documentStore.OpenSession())
            {
                totalCount = session.Query<User>().Count(u => u.ConfirmedAt != null);
            }

            Console.WriteLine($"{totalCount:N0} {site} users to import...");

            while (moreToRead)
                using (var session = documentStore.OpenSession())
                {
                    var users = session.Query<User>().Where(u => u.ConfirmedAt != null).Skip(skip).Take(take).ToArray();

                    foreach (var user in users)
                    {
                        if (site == "croquetscores.com")
                        {
                            var emailAddress = GetUniqueEmailAddress(connection, user.EmailAddress);

                            ValidateStringFieldLengths(site, user, emailAddress);
                            ImportUser(connection, site, user, emailAddress);
                        }
                        else
                        {
                            var emailAddress = user.EmailAddress;

                            if (EmailAddressExists(connection, emailAddress))
                            {
                                if (EmailAddressAndGateballRavenDbKeyExists(connection, emailAddress))
                                {
                                    emailAddress = GetUniqueEmailAddress(connection, user.EmailAddress);
                                }
                                else
                                {
                                    UpdateGateballRavenDbKey(connection, emailAddress, user.Id);
                                    continue;
                                }
                            }
                            ValidateStringFieldLengths(site, user, emailAddress);
                            ImportUser(connection, site, user, emailAddress);
                        }
                    }

                    skip += users.Length;
                    moreToRead = users.Length > 0;

                    Console.WriteLine($"Imported {skip:N0} {site} users of {totalCount:N0}...");
                }
        }

        private static void UpdateGateballRavenDbKey(MySqlConnection connection, string emailAddress, string userId)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Users SET GateballScoresRavenDbKey = @GateballScoresRavenDbKey WHERE EmailAddress = @EmailAddress;";
                command.Parameters.AddWithValue("@GateballScoresRavenDbKey", $"gateballscores.com/{userId}");
                command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                command.ExecuteNonQuery();

            }
        }

        private static bool EmailAddressAndGateballRavenDbKeyExists(MySqlConnection connection, string emailAddress)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Count(*) FROM Users WHERE EmailAddress = @EmailAddress AND GateballScoresRavenDbKey IS NOT NULL;";
                command.Parameters.AddWithValue("@EmailAddress", emailAddress);

                var count = command.ExecuteScalar();

                return (long)count > 0;
            }
        }

        private static string GetUniqueEmailAddress(MySqlConnection connection, string userEmailAddress)
        {
            if (!EmailAddressExists(connection, userEmailAddress))
            {
                return userEmailAddress;
            }

            var duplicate = 1;
            while (true)
            {
                var tryNotDuplicatedEmailAddress = $"duplicate_{duplicate}_{userEmailAddress}";

                if (!EmailAddressExists(connection, tryNotDuplicatedEmailAddress))
                {
                    Console.WriteLine($"Using email address {tryNotDuplicatedEmailAddress}.");
                    return tryNotDuplicatedEmailAddress;
                }

                duplicate++;
            }
        }

        private static void ImportUser(MySqlConnection connection, string site, User user, string emailAddress)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (" +
                                      "UserKey," +
                                      "Name," +
                                      "EmailAddress," +
                                      "Slug," +
                                      "Password," +
                                      "ConfirmKey," +
                                      "ConfirmedAt," +
                                      "LastSignIn," +
                                      "LastSignOut," +
                                      "FailedSignInAttempts," +
                                      "IsArchived," +
                                      "CroquetScoresRavenDbKey," +
                                      "GateballScoresRavenDbKey," +
                                      "Created," +
                                      "LastUpdate)" +
                                      " VALUES (" +
                                      "@UserKey," +
                                      "@Name," +
                                      "@EmailAddress," +
                                      "@Slug," +
                                      "@Password," +
                                      "@ConfirmKey," +
                                      "@ConfirmedAt," +
                                      "@LastSignIn," +
                                      "@LastSignOut," +
                                      "@FailedSignInAttempts," +
                                      "@IsArchived," +
                                      "@CroquetScoresRavenDbKey," +
                                      "@GateballScoresRavenDbKey," +
                                      "@Created," +
                                      "@LastUpdate)";

                command.Parameters.AddWithValue("@UserKey", Guid.NewGuid());
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                command.Parameters.AddWithValue("@Slug", user.Slug);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@ConfirmKey", user.ConfirmKey);
                command.Parameters.AddWithValue("@ConfirmedAt", user.ConfirmedAt);
                command.Parameters.AddWithValue("@LastSignIn", user.Authentication.LastSignIn);
                command.Parameters.AddWithValue("@LastSignOut", user.Authentication.LastSignOut);
                command.Parameters.AddWithValue("@FailedSignInAttempts", user.Authentication.FailedSignInAttempts);
                command.Parameters.AddWithValue("@IsArchived", user.IsArchived);
                command.Parameters.AddWithValue("@Created", new DateTime(2024, 1, 1));
                command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));

                if (site == "croquetscores.com")
                {
                    command.Parameters.AddWithValue("@CroquetScoresRavenDbKey", $"{site}/{user.Id}");
                    command.Parameters.AddWithValue("@GateballScoresRavenDbKey", null);
                }
                else
                {
                    command.Parameters.AddWithValue("@CroquetScoresRavenDbKey", null);
                    command.Parameters.AddWithValue("@GateballScoresRavenDbKey", $"{site}/{user.Id}");
                }

                command.ExecuteNonQuery();
            }
        }

        private static bool EmailAddressExists(MySqlConnection connection, string userEmailAddress)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Count(*) FROM Users WHERE EmailAddress = @EmailAddress;";
                command.Parameters.AddWithValue("@EmailAddress", userEmailAddress);

                var count = command.ExecuteScalar();

                return (long)count > 0;
            }
        }

        private static void ValidateStringFieldLengths(string site, User user, string emailAddress)
        {
            _maxNameLength = Math.Max(_maxNameLength, user.Name.Length);
            _maxEmailAddressLength = Math.Max(_maxEmailAddressLength, emailAddress.Length);
            _maxSlugLength = Math.Max(_maxSlugLength, user.Slug.Length);

            if (_maxNameLength > 1000)
            {
                throw new Exception(
                    $"{site}/{user.Id} name at {user.Name.Length:N0} characters is too long. {user.Name}");
            }

            if (_maxEmailAddressLength > 500)
            {
                throw new Exception(
                    $"{site}/{user.Id} email address at {emailAddress.Length:N0} characters is too long. {emailAddress}");
            }

            if (_maxSlugLength > 1000)
            {
                throw new Exception(
                    $"{site}/{user.Id} slug at {user.Slug.Length:N0} characters is too long. {user.Slug}");
            }
        }
    }
}