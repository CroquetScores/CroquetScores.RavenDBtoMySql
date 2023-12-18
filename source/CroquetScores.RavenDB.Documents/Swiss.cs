using System.Collections.Generic;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    // ReSharper disable once UnusedMember.Global
    public class Swiss : Competition
    {
        public List<Reference> CarryForwardCompetitions { get; set; }
    }
}