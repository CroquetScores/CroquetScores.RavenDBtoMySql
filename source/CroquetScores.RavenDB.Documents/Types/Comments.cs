using System.Collections.ObjectModel;

namespace CroquetScores.RavenDB.Documents.Types
{
    public class Comments : Collection<Comment>
    {
        public Comments()
        {
            // RavenDB requires a parameter less constructor.
        }
    }
}