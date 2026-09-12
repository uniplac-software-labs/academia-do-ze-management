using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Data;

public static partial class DataReaderExtensionsV2
{
    public static int GetInt32Value(this DbDataReader reader, string columnName)
        => Convert.ToInt32(reader.GetValue(reader.GetOrdinal(columnName)));

    public static string GetStringValue(this DbDataReader reader, string columnName)
        => Convert.ToString(reader.GetValue(reader.GetOrdinal(columnName))) ?? string.Empty;

    public static string GetNullableString(this DbDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? string.Empty : Convert.ToString(reader.GetValue(ordinal)) ?? string.Empty;
    }

    public static DateOnly GetDateOnlyValue(this DbDataReader reader, string columnName)
    {
        var value = reader.GetValue(reader.GetOrdinal(columnName));
        return value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            string s => DateOnly.Parse(s),
            _ => DateOnly.FromDateTime(Convert.ToDateTime(value))
        };
    }

    public static byte[]? GetNullableBytes(this DbDataReader reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        return reader.IsDBNull(ordinal) ? null : (byte[])reader.GetValue(ordinal);
    }
}