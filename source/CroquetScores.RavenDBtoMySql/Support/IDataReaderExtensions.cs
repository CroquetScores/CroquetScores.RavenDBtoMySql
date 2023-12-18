using System.Data;

namespace CroquetScores.RavenDBtoMySql.Support
{
    // ReSharper disable once InconsistentNaming
    public static class IDataReaderExtensions
    {
        /// <summary>Gets the string value of the specified field.</summary>
        /// <param name="dataRecord">The datareader this extension method is for.</param>
        /// <param name="field">The index of the field to find.</param>
        /// <returns>The string value of the specified field.</returns>
        /// <exception cref="T:System.IndexOutOfRangeException">
        ///     The index passed was outside the range of 0 through
        ///     <see cref="P:System.Data.IDataRecord.FieldCount" />.
        /// </exception>
        public static string GetNullableString(this IDataRecord dataRecord, int field)
        {
            return dataRecord.IsDBNull(field) ? null : dataRecord.GetString(field);
        }
    }
}