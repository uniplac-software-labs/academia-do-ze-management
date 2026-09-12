using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlConnector;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(DatabaseType dbType, string connectionString, CancellationToken cancellationToken = default)
    {
        string scriptName = dbType switch
        {
            DatabaseType.Sqlite => "script_sqlite.sql",
            DatabaseType.SqlServer => "script_sqlserver.sql",
            DatabaseType.MySql => "script_mysql.sql",
            _ => throw new ArgumentOutOfRangeException(nameof(dbType))
        };
        string scriptContent = ReadEmbeddedScript(scriptName);

        try
        {
            switch (dbType)
            {
                case DatabaseType.Sqlite:
                    using (var connection = new SqliteConnection(connectionString))
                    {
                        await connection.OpenAsync(cancellationToken);
                        using var command = connection.CreateCommand();
                        command.CommandText = scriptContent;
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                    break;
                case DatabaseType.SqlServer:
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync(cancellationToken);
                        using var command = connection.CreateCommand();
                        command.CommandText = scriptContent;
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                    break;
                case DatabaseType.MySql:
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync(cancellationToken);
                        foreach (var statement in scriptContent.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        {
                            using var command = connection.CreateCommand();
                            command.CommandText = statement;
                            await command.ExecuteNonQueryAsync(cancellationToken);
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            throw new InfrastructureException($"Erro ao inicializar o banco de dados ({dbType}): {ex.Message}", ex);
        }
    }

    private static string ReadEmbeddedScript(string scriptFileName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"AcademiaDoZe.Infrastructure.Scripts.{scriptFileName}";

        using Stream? stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new InfrastructureException($"Script SQL não encontrado nos recursos embutidos: {resourceName}");

        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}