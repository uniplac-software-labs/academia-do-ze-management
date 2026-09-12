using System.Data;
using System.Data.Common;
using AcademiaDoZe.Infrastructure.Exceptions;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbCommandExtensions
{
    public static void AddParameter(this DbCommand command, string name, object? value, DbType dbType)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.DbType = dbType;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    public static async Task<int> ExecuteScalarIdAsync(this DbCommand command, string errorCode, string errorMessage, CancellationToken cancellationToken = default)
    {
        var result = await command.ExecuteScalarAsync(cancellationToken);
        if (result == null || result == DBNull.Value)
            throw new InfrastructureException(errorCode, errorMessage);
        return Convert.ToInt32(result);
    }
}