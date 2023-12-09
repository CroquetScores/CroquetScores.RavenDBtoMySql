using System.ComponentModel.DataAnnotations;
using CroquetScores.RavenDB.Documents.Core;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class Commentary : RavenDocument
    {

        public Tournament.Reference Tournament { get; set; } = null!;

        public User.Reference Commentator { get; set; } = null!;

        public string Title { get; set; } = null!;

        [Required]
        public Comments Comments { get; set; } = null!;

        public bool IsArchived { get; set; }

        public DateTime? LastUpdate { get; set; } = null!;
    }
}