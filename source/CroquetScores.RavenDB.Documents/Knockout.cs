using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    // ReSharper disable once UnusedMember.Global
    public class Knockout : Competition
    {
        public int Size { get; set; }

        public int MaximumRounds { get; set; }
    }
}