using System;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class User : RavenDocument
    {

        public string Slug { get; set; }

        public string EmailAddress { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public Guid ConfirmKey { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        public Authentication Authentication { get; set; }

        public bool IsArchived { get; set; }
        public class Reference : RavenDocumentReference
        {
            public string Name { get; set; }
        }
    }
}