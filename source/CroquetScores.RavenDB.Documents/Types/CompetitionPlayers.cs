using System.Collections.Generic;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class CompetitionPlayers
    {
        private List<CompetitionPlayer> _Players { get; set; } = new List<CompetitionPlayer>();
    }
}