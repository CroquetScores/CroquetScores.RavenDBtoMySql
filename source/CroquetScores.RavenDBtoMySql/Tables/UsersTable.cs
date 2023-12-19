using System;
using CroquetScores.RavenDBtoMySql.Importers;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class UsersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating users table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE users (" +
                                      "UserKey CHAR(36) NOT NULL," +
                                      "Name VARCHAR(200) NOT NULL," +
                                      "EmailAddress VARCHAR(200) NOT NULL," +
                                      "Slug VARCHAR(200) NOT NULL," +
                                      "Password CHAR(52) NOT NULL," +
                                      "ConfirmKey CHAR(36) NOT NULL," +
                                      "ConfirmedAt DATETIME NULL," +
                                      "LastSignIn DATETIME NULL," +
                                      "LastSignOut DATETIME NULL," +
                                      "FailedSignInAttempts INT NULL," +
                                      "IsArchived TINYINT NOT NULL," +
                                      "CroquetScoresRavenDbKey VARCHAR(50) NULL," +
                                      "GateballScoresRavenDbKey VARCHAR(50) NULL," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL," +
                                      "PRIMARY KEY (UserKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE users" +
                                      " ADD UNIQUE INDEX EmailAddress_UNIQUE (EmailAddress ASC) VISIBLE;";
                command.ExecuteNonQuery();
            }
        }

        public static MySqlCommand GetUserKeyCroquetScoresRavenDbKeyCommand(
            MySqlConnection connection,
            string croquetScoresRavenDbKey)
        {
            var command = connection.CreateCommand();

            command.CommandText =
                "SELECT UserKey FROM users WHERE CroquetScoresRavenDbKey = @CroquetScoresRavenDbKey";

            command.Parameters.AddWithValue("@CroquetScoresRavenDbKey", croquetScoresRavenDbKey);

            return command;
        }

        public static MySqlCommand GetUserKeyGateballScoresRavenDbKeyCommand(
            MySqlConnection connection, string gateballScoresRavenDbKey)
        {
            var command = connection.CreateCommand();

            command.CommandText =
                "SELECT UserKey FROM users WHERE GateballScoresRavenDbKey = @GateballScoresRavenDbKey";

            command.Parameters.AddWithValue("@GateballScoresRavenDbKey", gateballScoresRavenDbKey);

            return command;
        }

        public static Guid GetUserKey(MySqlConnection connection, string site, string ravenDbId)
        {
            var result = TryGetUserKey(connection, site, ravenDbId);

            if (result != null)
            {
                return Guid.Parse(result.ToString());
            }

            Log.Warning($"Missing user {site}/{ravenDbId}.");
            return UsersImporter.MissingUserKey;
        }

        public static object TryGetUserKey(MySqlConnection connection, string site, string ravenDbId)
        {
            MySqlCommand command;

            switch (site)
            {
                case "croquetscores.com":
                    command = GetUserKeyCroquetScoresRavenDbKeyCommand(connection, ravenDbId);
                    break;

                case "gateballscores.com":
                    command = GetUserKeyGateballScoresRavenDbKeyCommand(connection, ravenDbId);
                    break;

                default:
                    throw new ArgumentException($"site {site} is not supported.", nameof(site));
            }

            var result = command.ExecuteScalar();

            command.Dispose();

            return result;
        }
    }
}