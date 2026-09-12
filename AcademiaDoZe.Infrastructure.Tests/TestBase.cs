using System;
using System.IO;
using System.Threading;
using AcademiaDoZe.Infrastructure.Data;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace AcademiaDoZe.Infrastructure.Tests;

public abstract class TestBase
{
    private static readonly string SqliteTestDatabase = Path.Combine(
        Path.GetTempPath(),
        $"academia_do_ze_tests_{Guid.NewGuid():N}.db");

    protected string ConnectionString { get; }
    protected DatabaseType DatabaseType { get; }
    protected static string DatabaseLabel => ResolveDatabaseType() switch
    {
        DatabaseType.Sqlite => "SQLite",
        DatabaseType.SqlServer => "SQLServer",
        DatabaseType.MySql => "MySQL",
        _ => throw new ArgumentOutOfRangeException(nameof(DatabaseLabel), "SGBD não suportado para testes.")
    };

    protected TestBase()
    {
        DatabaseType = ResolveDatabaseType();
        ConnectionString = DatabaseType switch
        {
            DatabaseType.SqlServer => @"Server=(localdb)\mssqllocaldb;Database=db_academia_do_ze;Trusted_Connection=True;TrustServerCertificate=True;",
            DatabaseType.MySql => Environment.GetEnvironmentVariable("ACADEMIA_MYSQL_CONNECTION_STRING")
                ?? "Server=localhost;Database=db_academia_do_ze;User Id=root;Password=;",
            DatabaseType.Sqlite => Environment.GetEnvironmentVariable("ACADEMIA_SQLITE_CONNECTION_STRING")
                ?? $"Data Source={SqliteTestDatabase};Cache=Shared;",
            _ => throw new ArgumentOutOfRangeException(nameof(DatabaseType), DatabaseType, "SGBD não suportado para testes.")
        };

        DbInitializer.InitializeAsync(DatabaseType, ConnectionString).GetAwaiter().GetResult();
    }

    private static DatabaseType ResolveDatabaseType()
    {
        string? configuredDatabase = Environment.GetEnvironmentVariable("ACADEMIA_DATABASE_TYPE");
        if (string.IsNullOrWhiteSpace(configuredDatabase))
            return DatabaseType.Sqlite;

        if (Enum.TryParse(configuredDatabase, ignoreCase: true, out DatabaseType databaseType))
            return databaseType;

        throw new ArgumentException(
            $"ACADEMIA_DATABASE_TYPE inválido: '{configuredDatabase}'. Use Sqlite, SqlServer ou MySql.");
    }

    private static long _counter = DateTime.UtcNow.Ticks % 8000000;
    protected static string GerarCep() => (80000000 + Interlocked.Increment(ref _counter) % 8000000).ToString("D8");

    private static long _cpfCounter = DateTime.UtcNow.Ticks % 9000000000L;
    protected static string GerarCpf() => (10000000000L + Interlocked.Increment(ref _cpfCounter)).ToString();

    private static long _telCounter = DateTime.UtcNow.Ticks % 90000000;
    protected static string GerarTelefone() => "119" + (10000000 + Interlocked.Increment(ref _telCounter) % 90000000).ToString("D8");

    protected static string GerarEmail() => $"user_{Guid.NewGuid():N}@test.com";
}
