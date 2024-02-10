using System;
using CroquetScores.RavenDBtoMySql.Support;
using MySql.Data.MySqlClient;

namespace CroquetScores.RavenDBtoMySql.Tables
{
    internal class PlayersTable
    {
        public static readonly Guid ByePlayerKey = Guid.Empty;
        private static int _maxNameLength;

        public static void CreateTable(MySqlConnection connection)
        {
            Log.Progress("Creating players table...");
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE players (" +
                                      "PlayerKey CHAR(36) NOT NULL," +
                                      "Name VARCHAR(200) NOT NULL," +
                                      "LastUpdate DATETIME NOT NULL, " +
                                      "PRIMARY KEY (PlayerKey));";
                command.ExecuteNonQuery();

                command.CommandText = "ALTER TABLE players " +
                                      "ADD INDEX Name (Name ASC) VISIBLE;";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO players (" +
                                      "PlayerKey, " +
                                      "Name, " +
                                      "LastUpdate) " +
                                      "VALUES (" +
                                      "@PlayerKey, " +
                                      "@Name, " +
                                      "@LastUpdate);";

                command.Parameters.AddWithValue("@PlayerKey", ByePlayerKey);
                command.Parameters.AddWithValue("@Name", "Bye");
                command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));

                command.ExecuteNonQuery();
            }
        }

        public static Guid FindOrAddByName(MySqlConnection connection, string name)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT PlayerKey " +
                                      "FROM players " +
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
                _maxNameLength = Math.Max(_maxNameLength, name.Length);

                if (name.Length > 200)
                {
                    name = name.Substring(0, 200);
                    Log.Error($"Player name is too long. {name}");
                }

                var playerKey = Guid.NewGuid();

                command.CommandText = "INSERT INTO players (" +
                                      "PlayerKey," +
                                      "Name," +
                                      "LastUpdate) " +
                                      "VALUES (" +
                                      "@PlayerKey," +
                                      "@Name," +
                                      "@LastUpdate)";

                command.Parameters.AddWithValue("@PlayerKey", playerKey);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@LastUpdate", new DateTime(2024, 1, 1));

                command.ExecuteNonQuery();

                return playerKey;
            }
        }

        public static void LogStatistics()
        {
            Log.Statistic($"Longest player name {_maxNameLength}");
        }
    }
}