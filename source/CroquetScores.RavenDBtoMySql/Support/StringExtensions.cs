namespace CroquetScores.RavenDBtoMySql.Support
{
    internal static class StringExtensions
    {
        public static int GetLength(this string value)
        {
            return value?.Length ?? 0;
        }
    }
}