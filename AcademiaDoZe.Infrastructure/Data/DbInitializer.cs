using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;

namespace AcademiaDoZe.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(DatabaseType dbType, string connectionString, CancellationToken cancellationToken = default)
    {
        string scriptName = dbType == DatabaseType.Sqlite ? "script_sqlite.sql" : "script_sqlserver.sql";
        string scriptContent = ReadEmbeddedScript(scriptName);

        try
        {
            if (dbType == DatabaseType.Sqlite)
            {
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                using var command = connection.CreateCommand();
                command.CommandText = scriptContent;
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            else
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                using var command = connection.CreateCommand();
                command.CommandText = scriptContent;
                await command.ExecuteNonQueryAsync(cancellationToken);
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