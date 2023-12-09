namespace CroquetScores.RavenDB.Documents.Types
{
    public class TournamentPlayer
    {

        public int _Id { get; set; }

        public string Name { get; set; }

        public string Representing { get; set; }

        public string Slug { get; set; }

        public class Reference
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Representing { get; set; }
        }
    }
}