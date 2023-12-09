using CroquetScores.RavenDB.Documents.Core;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents.Competitions
{
    public abstract class Competition : RavenDocument
    {
        public Tournament.Reference Tournament { get; set; } = null!;

        public string Name { get; set; } = null!;

        public CompetitionPlayers Players { get; set; } = null!;

        public CompetitionGames Games { get; set; } = null!;

        public bool IsArchived { get; set; }

        public bool? IsMajorCompetition { get; set; }

        public string Slug { get; set; } = null!;

        public class Reference : RavenDocumentReference
        {   
            public string Name { get; set; } = null!;

            public string Slug { get; set; } = null!;
        }
    }
}