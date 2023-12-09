using System.ComponentModel.DataAnnotations;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class Result
    {
        [Required]
        public int PlayerId { get; set; }

        [Required]
        public Score Score { get; set; }
    }
}