using System;
using System.Data;
using CroquetScores.RavenDB.Documents.Types;
using CroquetScores.RavenDBtoMySql.Support;

namespace CroquetScores.RavenDBtoMySql.TableRows
{
    internal class TournamentPlayer
    {
        public TournamentPlayer(IDataRecord reader)
        {
            TournamentPlayerKey = reader.GetGuid(0);
            TournamentKey = reader.GetGuid(1);
            Name = reader.GetString(2);
            Representing = reader.GetNullableString(3);
            Slug = reader.GetString(4);
            Created = reader.GetDateTime(5);
            LastUpdated = reader.GetDateTime(6);
            RavenDbKey = reader.GetInt32(7);
        }

        public TournamentPlayer(Guid tournamentPlayerKey, Guid tournamentKey, CompetitionPlayer competitionPlayer)
        {
            TournamentPlayerKey = tournamentPlayerKey;
            TournamentKey = tournamentKey;
            Name = competitionPlayer.Name;
            Representing = competitionPlayer.Representing;
            Slug = competitionPlayer.Slug;
            Created = new DateTime(2024, 1, 1);
            LastUpdated = new DateTime(2024, 1, 1);
            RavenDbKey = competitionPlayer._Id;
        }

        public Guid TournamentPlayerKey { get; private set; }
        public Guid TournamentKey { get; private set; }
        public string Name { get; private set; }
        public string Representing { get; private set; }
        public string Slug { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public int RavenDbKey { get; private set; }
    }
}