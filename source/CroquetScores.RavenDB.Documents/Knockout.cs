using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    // todo: fully define.
    public class Knockout : Competition
    {
        public int Size { get; set; }

        public int MaximumRounds { get; set; }
    }
}