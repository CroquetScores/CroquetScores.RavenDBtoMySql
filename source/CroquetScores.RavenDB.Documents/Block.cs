using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    // todo: fully define.
    public class Block : Competition
    {

        public int BestOf { get; set; } = 1;
    }
}