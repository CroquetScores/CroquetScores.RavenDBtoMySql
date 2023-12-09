using Microsoft.VisualBasic.CompilerServices;

namespace CroquetScores.RavenDB.Documents.Core
{
    public abstract class RavenDocument
    {

        public string Id { get; set; } = null!;
    }
}