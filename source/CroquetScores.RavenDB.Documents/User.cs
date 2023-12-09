using CroquetScores.RavenDB.Documents.Core;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class User : RavenDocument
    {

        public string Slug { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Password { get; set; } = null!;

        public Guid ConfirmKey { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        public Authentication Authentication { get; set; } = null!;

        public bool IsArchived { get; set; }
        public class Reference : RavenDocumentReference
        {
            public string Name { get; set; } = null!;
        }
    }
}