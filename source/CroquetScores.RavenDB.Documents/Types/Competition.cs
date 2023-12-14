namespace CroquetScores.RavenDB.Documents.Types
{
    public abstract class Competition : RavenDocument
    {
        public Tournament.Reference Tournament { get; set; }

        public string Name { get; set; }

        public CompetitionPlayers Players { get; set; }

        public CompetitionGames Games { get; set; }

        public bool IsArchived { get; set; }

        public bool? IsMajorCompetition { get; set; }

        public string Slug { get; set; }

        public class Reference : RavenDocumentReference
        {
            public string Name { get; set; }

            public string Slug { get; set; }
        }
    }
}