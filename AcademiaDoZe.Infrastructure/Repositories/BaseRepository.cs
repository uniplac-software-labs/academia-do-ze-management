using System;
using System.Data;
using System.Data.Common;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;

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

    // ---- Mantido para compatibilidade com LogradouroRepository ----
    protected IDbConnection CreateConnection()
    {
        return DatabaseType switch
        {
            DatabaseType.Sqlite => new SqliteConnection(ConnectionString),
            DatabaseType.SqlServer => new SqlConnection(ConnectionString),
            DatabaseType.MySql => new MySqlConnection(ConnectionString),
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

    // ---- Novo: padrão assíncrono usado por Colaborador/Aluno/Matricula ----
    protected async Task<DbCommand> CreateCommandAsync(string query, CancellationToken cancellationToken = default)
    {
        DbConnection connection = DatabaseType switch
        {
            DatabaseType.Sqlite => new SqliteConnection(ConnectionString),
            DatabaseType.SqlServer => new SqlConnection(ConnectionString),
            DatabaseType.MySql => new MySqlConnection(ConnectionString),
            _ => throw new InvalidOperationException($"SGBD não suportado: {DatabaseType}")
        };

        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = query;
        // Fecha a conexão automaticamente quando o command é disposed (await using)
        command.Disposed += (_, _) => connection.Dispose();
        return command;
    }

    protected string FormatInsertQuery(string insertStatement)
    {
        string idQuery = DatabaseType switch
        {
            DatabaseType.Sqlite => "SELECT last_insert_rowid();",
            DatabaseType.SqlServer => "SELECT CAST(SCOPE_IDENTITY() AS INT);",
            DatabaseType.MySql => "SELECT LAST_INSERT_ID();",
            _ => throw new InvalidOperationException($"SGBD não suportado: {DatabaseType}")
        };
        return $"{insertStatement.TrimEnd().TrimEnd(';')}; {idQuery}";
    }

    protected string GetCurrentDateFunction() => DatabaseType switch
    {
        DatabaseType.Sqlite => "date('now')",
        DatabaseType.SqlServer => "CAST(GETDATE() AS DATE)",
        DatabaseType.MySql => "CURDATE()",
        _ => throw new InvalidOperationException($"SGBD não suportado: {DatabaseType}")
    };

    protected string GetDateAddDaysExpression(string dateExpression, string daysParameter) => DatabaseType switch
    {
        DatabaseType.Sqlite => $"date({dateExpression}, {daysParameter} || ' days')",
        DatabaseType.SqlServer => $"DATEADD(day, {daysParameter}, {dateExpression})",
        DatabaseType.MySql => $"DATE_ADD({dateExpression}, INTERVAL {daysParameter} DAY)",
        _ => throw new InvalidOperationException($"SGBD não suportado: {DatabaseType}")
    };
}