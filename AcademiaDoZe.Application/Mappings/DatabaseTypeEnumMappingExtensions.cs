using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Infrastructure.Data;

namespace AcademiaDoZe.Application.Mappings;

public static class DatabaseTypeEnumMappingExtensions
{
    // Mapeamento explícito: os valores numéricos dos dois enums não são iguais (ex.: SqlServer = 0 em um e Sqlite = 0 no outro).
    public static DatabaseType ToInfrastructure(this AppDatabaseType appDatabaseType) => appDatabaseType switch
    {
        AppDatabaseType.SqlServer => DatabaseType.SqlServer,
        AppDatabaseType.MySql => DatabaseType.MySql,
        AppDatabaseType.Sqlite => DatabaseType.Sqlite,
        _ => throw new ArgumentOutOfRangeException(nameof(appDatabaseType), appDatabaseType, null)
    };

    public static AppDatabaseType ToApplication(this DatabaseType databaseType) => databaseType switch
    {
        DatabaseType.SqlServer => AppDatabaseType.SqlServer,
        DatabaseType.MySql => AppDatabaseType.MySql,
        DatabaseType.Sqlite => AppDatabaseType.Sqlite,
        _ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null)
    };
}
