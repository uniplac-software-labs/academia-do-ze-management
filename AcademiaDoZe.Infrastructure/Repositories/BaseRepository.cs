using System;
using System.Data;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace AcademiaDoZe.Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected string ConnectionString { get; }
    protected DatabaseType DatabaseType { get; }

    protected BaseRepository(string connectionString, DatabaseType databaseType)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        DatabaseType = databaseType;
    }

    protected IDbConnection CreateConnection()
    {
        return DatabaseType switch
        {
            DatabaseType.Sqlite => new SqliteConnection(ConnectionString),
            DatabaseType.SqlServer => new SqlConnection(ConnectionString),
            _ => throw new InvalidOperationException($"SGBD não suportado: {DatabaseType}")
        };
    }

    protected IDbCommand CreateCommand(string query, IDbConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = query;
        return command;
    }

    protected void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}