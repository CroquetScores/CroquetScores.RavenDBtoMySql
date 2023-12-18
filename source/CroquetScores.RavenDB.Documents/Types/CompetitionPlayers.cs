using System.Collections.Generic;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class CompetitionPlayers
    {
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once InconsistentNaming
        public List<CompetitionPlayer> _Players { get; set; } = new List<CompetitionPlayer>();
    }
}