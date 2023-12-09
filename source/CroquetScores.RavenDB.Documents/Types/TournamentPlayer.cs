namespace CroquetScores.RavenDB.Documents.Types
{
    public class TournamentPlayer
    {

        public int _Id { get; set; }

        public string Name { get; set; } = null!;

        public string Representing { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public class Reference
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public string Representing { get; set; } = null!;
        }
    }
}