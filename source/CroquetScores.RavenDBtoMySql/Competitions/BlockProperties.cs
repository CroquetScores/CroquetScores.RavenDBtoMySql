using CroquetScores.RavenDB.Documents;

namespace CroquetScores.RavenDBtoMySql.Competitions
{
    internal class BlockProperties
    {
        public BlockProperties(Block block)
        {
            BestOf = block.BestOf;
        }

        public int BestOf { get; private set; }
    }
}