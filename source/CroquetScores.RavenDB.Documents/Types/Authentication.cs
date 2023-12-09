using System;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class Authentication
    {
        public DateTime? LastSignIn { get; set; }
        public DateTime? LastSignOut { get; set; }
        public int FailedSignInAttempts { get; set; }
    }
}