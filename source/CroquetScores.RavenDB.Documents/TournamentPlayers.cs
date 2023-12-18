using System.Collections.Generic;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class TournamentPlayers : RavenDocument
    {
        // ReSharper disable once InconsistentNaming
        public List<TournamentPlayer> _Players { get; set; } = new List<TournamentPlayer>();

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once InconsistentNaming
        public int _LastId { get; set; }

        public Tournament.Reference Tournament { get; set; }

        public bool IsArchived { get; set; }
    }
}