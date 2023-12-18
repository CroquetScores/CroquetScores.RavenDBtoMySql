using System.Collections.Generic;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class CompetitionGames
    {
        // ReSharper disable once InconsistentNaming
        public List<Game> _Games { get; set; } = new List<Game>();

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public int _LastId { get; set; }
    }
}