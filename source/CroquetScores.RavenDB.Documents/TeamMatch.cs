using CroquetScores.RavenDB.Documents.Competitions;

namespace CroquetScores.RavenDB.Documents
{
    // todo: fully document.

    public class TeamMatch : Competition
    {

        public string TeamA { get; set; } = null!;

        public string TeamB { get; set; } = null!;

        public DateTime Start { get; set; }

        public DateTime Finish { get; set; }

        public int ExpectedMatches { get; set; }

        public int BestOf { get; set; }

        public string Group { get; set; } = null!;
    }
}