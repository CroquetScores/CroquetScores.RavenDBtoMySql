// ReSharper disable UnusedMember.Global

namespace CroquetScores.RavenDB.Documents.Types
{
    // todo: why inherit from player? why not use player.Reference?
    public class CompetitionPlayer : TournamentPlayer
    {
        public CarryForwardProperties CarryForward { get; set; } = new CarryForwardProperties();

        public bool IsWithdrawn { get; set; }

        public class CarryForwardProperties
        {
            public int MatchWins { get; set; }

            public int MatchLosses { get; set; }

            public int GameWins { get; set; }

            public int GameLosses { get; set; }

            public int ForHoops { get; set; }

            public int AgainstHoops { get; set; }
        }
    }
}