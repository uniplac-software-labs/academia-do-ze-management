using System;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DataReaderExtensions
{
    public static T GetValue<T>(this System.Data.IDataRecord reader, string columnName)
    {
        int ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal))
            return default!;

        object value = reader.GetValue(ordinal);
        return (T)Convert.ChangeType(value, typeof(T));
    }
}