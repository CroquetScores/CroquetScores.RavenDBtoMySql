using System.Collections.Generic;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class CompetitionGames
    {
        private List<Game> _Games { get; set; } = new List<Game>();

        private int _LastId { get; set; }
    }
}