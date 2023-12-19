﻿using System;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class PlayersTable
    {
        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating Players table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE Players (" +
                                      "PlayerKey CHAR(36) NOT NULL," +
                                      "Name VARCHAR(500)," +
                                      "Created DATETIME NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL, " +
                                      "PRIMARY KEY (PlayerKey));";

                command.ExecuteNonQuery();
            }
        }

        public static Guid FindOrAddByName(MySqlConnection connection, string name)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT PlayerKey " +
                                      "FROM Players " +
                                      "WHERE Name = @Name;";

                command.Parameters.AddWithValue("name", name);

                var playerKey = command.ExecuteScalar();

                if (playerKey != null)
                {
                    return Guid.Parse(playerKey.ToString());
                }
            }

            using (var command = connection.CreateCommand())
            {
                var playerKey = Guid.NewGuid();

                command.CommandText = "INSERT INTO Players (" +
                                      "PlayerKey," +
                                      "Name," +
                                      "Created," +
                                      "LastUpdate) " +
                                      "VALUES (" +
                                      "@PlayerKey," +
                                      "@Name," +
                                      "@Created," +
                                      "@LastUpdate)";

                command.Parameters.AddWithValue("@PlayerKey", playerKey);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Created", new DateTime(2024, 1, 1));
                command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));

                command.ExecuteNonQuery();

                return playerKey;
            }
        }
    }
}