using System;
using System.Linq;
using CroquetScores.RavenDB.Documents;
using CroquetScores.RavenDB.Documents.Types;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;
using Raven.Client;

namespace CroquetScores.RavenDBtoMySql.Importers
{
    internal class UsersImporter
    {
        private static int _maxNameLength;
        private static int _maxEmailAddressLength;
        private static int _maxSlugLength;
        internal static Guid MissingUserKey;

        public static void Import(IDocumentStore documentStore, MySqlConnection connection, string site)
        {
            Log.Progress("Importing users data...");

            int totalCount;
            var skip = 0;
            const int take = 100;
            var moreToRead = true;

            using (var session = documentStore.OpenSession())
            {
                totalCount = session.Query<User>().Count(u => u.ConfirmedAt != null);
            }

            Log.Statistic($"{totalCount:N0} {site} users to import...");

            using (var insertCommand = CreateInsertCommand(connection))
            {
                if (site == "croquetscores.com")
                {
                    var missingUser = CreateMissingUser();
                    MissingUserKey = ImportUser(insertCommand, "croquetscores.com", missingUser,
                        missingUser.EmailAddress);
                }

                while (moreToRead)
                {
                    using (var session = documentStore.OpenSession())
                    {
                        var users = session
                            .Query<User>()
                            .Where(u => u.ConfirmedAt != null)
                            .Skip(skip)
                            .Take(take)
                            .ToArray();

                        foreach (var user in users)
                        {
                            if (site == "croquetscores.com")
                            {
                                var emailAddress = GetUniqueEmailAddress(connection, user.EmailAddress);

                                ValidateStringFieldLengths(site, user, emailAddress);
                                ImportUser(insertCommand, site, user, emailAddress);
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
                                ImportUser(insertCommand, site, user, emailAddress);
                            }
                        }

                        skip += users.Length;
                        moreToRead = users.Length > 0 && Program.ReadAll;

                        Log.Progress($"Imported {skip:N0} {site} users of {totalCount:N0}...");
                    }
                }
            }

            Log.Statistic($"Max email address length: {_maxEmailAddressLength}");
            Log.Statistic($"Max name length: {_maxNameLength}");
            Log.Statistic($"Max slug length: {_maxSlugLength}");
        }

        public static Guid Import(MySqlConnection connection, string site, User.Reference tournamentManager)
        {
            var emailAddress =
                GetUniqueEmailAddress(connection, $"missing-user-record-{tournamentManager.Name}@example.com");
            var user = new User
            {
                EmailAddress = emailAddress,
                Authentication = new Authentication(),
                ConfirmKey = Guid.NewGuid(),
                Id = tournamentManager.Id,
                IsArchived = true,
                Name = tournamentManager.Name,
                Password = "fake",
                Slug = tournamentManager.Name.ToLower().Replace(" ", "-")
            };

            ValidateStringFieldLengths(site, user, emailAddress);

            using (var insertCommand = CreateInsertCommand(connection))
            {
                return ImportUser(insertCommand, site, user, emailAddress);
            }
        }

        private static User CreateMissingUser()
        {
            return new User
            {
                Name = "Missing on transfer from RavenDB to MySQL",
                Slug = "missing-on-transfer-from-ravendb-to-mysql",
                EmailAddress = "missing@example.com",
                ConfirmKey = Guid.NewGuid(),
                Authentication = new Authentication(),
                Id = "users/0",
                IsArchived = true,
                Password = "missing"
            };
        }

        private static MySqlCommand CreateInsertCommand(MySqlConnection connection)
        {
            var command = connection.CreateCommand();

            command.CommandText =
                "INSERT INTO Users (" +
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

            command.Parameters.AddWithValue("@UserKey", null);
            command.Parameters.AddWithValue("@Name", null);
            command.Parameters.AddWithValue("@EmailAddress", null);
            command.Parameters.AddWithValue("@Slug", null);
            command.Parameters.AddWithValue("@Password", null);
            command.Parameters.AddWithValue("@ConfirmKey", null);
            command.Parameters.AddWithValue("@ConfirmedAt", null);
            command.Parameters.AddWithValue("@LastSignIn", null);
            command.Parameters.AddWithValue("@LastSignOut", null);
            command.Parameters.AddWithValue("@FailedSignInAttempts", null);
            command.Parameters.AddWithValue("@IsArchived", null);
            command.Parameters.AddWithValue("@Created", new DateTime(2024, 1, 1));
            command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));
            command.Parameters.AddWithValue("@CroquetScoresRavenDbKey", null);
            command.Parameters.AddWithValue("@GateballScoresRavenDbKey", null);

            return command;
        }

        private static void UpdateGateballRavenDbKey(MySqlConnection connection, string emailAddress, string userId)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "UPDATE Users SET GateballScoresRavenDbKey = @GateballScoresRavenDbKey WHERE EmailAddress = @EmailAddress;";
                command.Parameters.AddWithValue("@GateballScoresRavenDbKey", $"{userId}");
                command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                command.ExecuteNonQuery();
            }
        }

        private static bool EmailAddressAndGateballRavenDbKeyExists(MySqlConnection connection, string emailAddress)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText =
                    "SELECT Count(*) FROM Users WHERE EmailAddress = @EmailAddress AND GateballScoresRavenDbKey IS NOT NULL;";
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
                    Log.Warning($"Using email address {tryNotDuplicatedEmailAddress}.");
                    return tryNotDuplicatedEmailAddress;
                }

                duplicate++;
            }
        }

        private static Guid ImportUser(MySqlCommand command, string site, User user, string emailAddress)
        {
            var userKey = Guid.NewGuid();

            command.Parameters["@UserKey"].Value = userKey;
            command.Parameters["@Name"].Value = user.Name;
            command.Parameters["@EmailAddress"].Value = emailAddress;
            command.Parameters["@Slug"].Value = user.Slug;
            command.Parameters["@Password"].Value = user.Password;
            command.Parameters["@ConfirmKey"].Value = user.ConfirmKey;
            command.Parameters["@ConfirmedAt"].Value = user.ConfirmedAt;
            command.Parameters["@LastSignIn"].Value = user.Authentication.LastSignIn;
            command.Parameters["@LastSignOut"].Value = user.Authentication.LastSignOut;
            command.Parameters["@FailedSignInAttempts"].Value = user.Authentication.FailedSignInAttempts;
            command.Parameters["@IsArchived"].Value = user.IsArchived;

            if (site == "croquetscores.com")
            {
                command.Parameters["@CroquetScoresRavenDbKey"].Value = user.Id;
                command.Parameters["@GateballScoresRavenDbKey"].Value = null;
            }
            else
            {
                command.Parameters["@CroquetScoresRavenDbKey"].Value = null;
                command.Parameters["@GateballScoresRavenDbKey"].Value = user.Id;
            }

            command.ExecuteNonQuery();

            return userKey;
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

            if (_maxNameLength > 200)
            {
                Log.Error($"{site}/{user.Id} name at {user.Name.Length:N0} characters is too long. {user.Name}");
                user.Name = user.Name.Substring(0, 200);
            }

            if (_maxEmailAddressLength > 200)
            {
                Log.Error($"{site}/{user.Id} email address at {emailAddress.Length:N0} characters is too long. {emailAddress}");
                user.EmailAddress = user.EmailAddress.Substring(0, 200);
            }

            if (_maxSlugLength <= 200)
            {
                return;
            }

            Log.Error($"{site}/{user.Id} slug at {user.Slug.Length:N0} characters is too long. {user.Slug}");
            user.Slug = user.Slug.Substring(0, 200);
        }
    }
}