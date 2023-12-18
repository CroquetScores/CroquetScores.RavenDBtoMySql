using System.Linq;

namespace CroquetScores.RavenDB.Documents.Types
{
    public static class CharExtensions
    {
        public static bool IsInteger(this char value)
        {
            return "0123456789".Contains(value);
        }
    }
}