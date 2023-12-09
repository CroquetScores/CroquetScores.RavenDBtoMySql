using CroquetScores.RavenDB.Documents.Competitions;
using CroquetScores.RavenDB.Documents.Core;

namespace CroquetScores.RavenDB.Documents
{
    public class Tournament : RavenDocument
    {
        public string Name { get; set; } = null!;

        public string Slug { get; set; } = null!;

        public DateTime Start { get; set; }

        public DateTime Finish { get; set; }

        public User.Reference CreatedBy { get; set; } = null!;

        public HashSet<User.Reference> Managers { get; set; } = new();

        public HashSet<User.Reference> Scorers { get; set; } = new();

        public List<Competition.Reference> Competitions { get; set; } = new();

        public bool IsArchived { get; set; }

        public bool? IsMajorTournament { get; set; }

        public string SportType { get; set; } = null!;

        public string TournamentType { get; set; } = null!;

        public string TimeZoneId { get; set; } = null!;

        public class Reference : RavenDocumentReference
        {
            public string Name { get; set; } = null!;
        }
    }
}