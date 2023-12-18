using CroquetScores.RavenDB.Documents;

namespace CroquetScores.RavenDBtoMySql.Competitions
{
    internal class KnockoutProperties
    {
        public KnockoutProperties(Knockout knockout)
        {
            Size = knockout.Size;
        }

        public int Size { get; private set; }
    }
}