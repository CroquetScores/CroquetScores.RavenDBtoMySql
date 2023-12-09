using System.Collections;
using System.ComponentModel.DataAnnotations;
using CroquetScores.RavenDB.Documents.Core;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class TournamentPlayers : RavenDocument
    {

        public List<TournamentPlayer> _Players { get; set; } = new List<TournamentPlayer>();

        public int _LastId { get; set; }

        public Tournament.Reference Tournament { get; set; } = null!;

        public bool IsArchived { get; set; }
    }
}