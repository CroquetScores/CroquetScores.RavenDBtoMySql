using System;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class UsersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Console.WriteLine("Creating Users table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Users (" +
                                      "UserKey CHAR(36) NOT NULL," +
                                      "Name VARCHAR(1000) NOT NULL," +
                                      "EmailAddress VARCHAR(500) NOT NULL," +
                                      "Slug VARCHAR(1000) NOT NULL," +
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

                command.CommandText = "ALTER TABLE Users" +
                                      " ADD UNIQUE INDEX EmailAddress_UNIQUE (EmailAddress ASC) VISIBLE;";
                command.ExecuteNonQuery();
            }
        }
    }
}