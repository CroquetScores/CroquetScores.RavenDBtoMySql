using System;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class Commentary : RavenDocument
    {

        public Tournament.Reference Tournament { get; set; }

        public User.Reference Commentator { get; set; }

        public string Title { get; set; }

        public Comments Comments { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? LastUpdate { get; set; }
    }
}