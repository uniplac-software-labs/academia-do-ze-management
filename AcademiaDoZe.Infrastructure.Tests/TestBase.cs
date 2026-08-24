using System;
using System.Threading;
using AcademiaDoZe.Infrastructure.Data;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace AcademiaDoZe.Infrastructure.Tests;

public abstract class TestBase
{
    private const DatabaseType SelectedDatabaseType = DatabaseType.SqlServer;

    protected string ConnectionString { get; }
    protected DatabaseType DatabaseType { get; }

    protected TestBase()
    {
        DatabaseType = SelectedDatabaseType;
        ConnectionString = DatabaseType switch
        {
            DatabaseType.SqlServer => @"Server=(localdb)\mssqllocaldb;Database=db_academia_do_ze;Trusted_Connection=True;TrustServerCertificate=True;",
            DatabaseType.Sqlite => "Data Source=db_academia_do_ze.db;Cache=Shared;",
            _ => throw new ArgumentOutOfRangeException(nameof(DatabaseType), DatabaseType, "SGBD não suportado para testes.")
        };

        DbInitializer.InitializeAsync(DatabaseType, ConnectionString).GetAwaiter().GetResult();
    }

    private static int _counter = 10000;
    protected static string GerarCep() => (80000000 + ((int)(DateTime.UtcNow.Ticks % 8000000)) + Interlocked.Increment(ref _counter)).ToString("D8")[..8];
}