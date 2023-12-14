using System;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class Comment
    {
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsArchived { get; set; }

        public int? Readers { get; set; }
    }
}