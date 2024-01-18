using System;
using System.Data;
using CroquetScores.RavenDBtoMySql.Support;

namespace CroquetScores.RavenDBtoMySql.TableRows
{
    internal class CompetitionPlayerRow
    {
        public CompetitionPlayerRow(IDataRecord reader)
        {
            CompetitionPlayerKey = reader.GetGuid(0);
            CompetitionKey = reader.GetGuid(1);
            PlayerKey = reader.GetGuid(2);
            OrderBy =reader.GetInt32(3);
            Representing = reader.GetNullableString(4);
            Slug = reader.GetString(5);
            LastUpdated = reader.GetDateTime(6);
            RavenDbKey = reader.GetInt32(7);
        }

        public CompetitionPlayerRow(Guid competitionPlayerKey, Guid competitionKey, Guid playerKey, int orderBy, RavenDB.Documents.Types.CompetitionPlayer ravenCompetitionPlayer)
        {
            CompetitionPlayerKey = competitionPlayerKey;
            CompetitionKey = competitionKey;
            PlayerKey = playerKey;
            OrderBy = orderBy;
            Representing = ravenCompetitionPlayer.Representing;
            Slug = ravenCompetitionPlayer.Slug;
            LastUpdated = new DateTime(2024, 1, 1);
            RavenDbKey = ravenCompetitionPlayer._Id;
        }

        public Guid CompetitionPlayerKey { get; private set; }
        public Guid CompetitionKey { get; private set; }
        public Guid PlayerKey { get; private set; }
        public int OrderBy { get; private set; }
        public string Representing { get; set; }
        public string Slug { get; private set; }
        public DateTime LastUpdated { get; private set; }
        public int RavenDbKey { get; private set; }
    }
}