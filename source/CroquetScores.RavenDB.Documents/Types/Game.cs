using System.ComponentModel.DataAnnotations;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class Game
    {
        public int Id { get; set; }

        public Result Winner { get; set; } = null!;

        public Result Loser { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}