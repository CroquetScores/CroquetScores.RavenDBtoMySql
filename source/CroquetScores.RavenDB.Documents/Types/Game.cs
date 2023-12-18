// ReSharper disable UnusedMember.Global

using System;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class Game
    {
        public int Id { get; set; }

        public Result Winner { get; set; }

        public Result Loser { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}