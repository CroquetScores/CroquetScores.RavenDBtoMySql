using CroquetScores.RavenDB.Documents.Competitions;

namespace CroquetScores.RavenDB.Documents
{
    // todo: fully define.
    public class Swiss : Competition
    {
        public Swiss()
        {
            CarryForwardCompetitions = new List<Reference>();
        }

        public List<Reference> CarryForwardCompetitions { get; set; }
    }
}