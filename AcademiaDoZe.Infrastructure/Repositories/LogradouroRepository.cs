using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Linq;
namespace AcademiaDoZe.Infrastructure.Repositories;

public class LogradouroRepository : BaseRepository, ILogradouroRepository
{
    public LogradouroRepository(string connectionString, DatabaseType databaseType)
        : base(connectionString, databaseType) { }

    public async Task<Logradouro?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT id_logradouro, cep, nome, bairro, cidade, estado, pais FROM tb_logradouro WHERE id_logradouro = @id;";

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@id", id);

        using var reader = (IDataReader)await Task.Run(() => command.ExecuteReader(), cancellationToken);
        if (reader.Read())
        {
            return MapToDomain(reader);
        }

        return null;
    }

    public async Task<IEnumerable<Logradouro>> ObterTodos(CancellationToken cancellationToken = default)
    {
        const string query = "SELECT id_logradouro, cep, nome, bairro, cidade, estado, pais FROM tb_logradouro ORDER BY nome;";
        var list = new List<Logradouro>();

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        using var reader = (IDataReader)await Task.Run(() => command.ExecuteReader(), cancellationToken);

        while (reader.Read())
        {
            list.Add(MapToDomain(reader));
        }

        return list;
    }

    public async Task<Logradouro> Adicionar(Logradouro entity, CancellationToken cancellationToken = default)
    {
        string query = DatabaseType == DatabaseType.Sqlite
            ? @"INSERT INTO tb_logradouro (cep, nome, bairro, cidade, estado, pais)
                VALUES (@cep, @nome, @bairro, @cidade, @estado, @pais);
                SELECT last_insert_rowid();"
            : @"INSERT INTO tb_logradouro (cep, nome, bairro, cidade, estado, pais)
                VALUES (@cep, @nome, @bairro, @cidade, @estado, @pais);
                SELECT SCOPE_IDENTITY();";

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@cep", entity.Cep.Valor);
        AddParameter(command, "@nome", entity.Nome);
        AddParameter(command, "@bairro", entity.Bairro);
        AddParameter(command, "@cidade", entity.Cidade);
        AddParameter(command, "@estado", entity.Estado);
        AddParameter(command, "@pais", entity.Pais);

        object? result = await Task.Run(() => command.ExecuteScalar(), cancellationToken);
        int newId = Convert.ToInt32(result);

        var createdResult = Logradouro.Criar(newId, entity.Cep.Valor, entity.Nome, entity.Bairro, entity.Cidade, entity.Estado, entity.Pais);
        if (createdResult.IsFailure)
            throw new InfrastructureException("Erro ao reconstituir entidade Logradouro após inserção.");

        return createdResult.Value!;
    }

    public async Task<Logradouro> Atualizar(Logradouro entity, CancellationToken cancellationToken = default)
    {
        const string query = @"UPDATE tb_logradouro 
                              SET cep = @cep, nome = @nome, bairro = @bairro, cidade = @cidade, estado = @estado, pais = @pais 
                              WHERE id_logradouro = @id;";

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@id", entity.Id);
        AddParameter(command, "@cep", entity.Cep.Valor);
        AddParameter(command, "@nome", entity.Nome);
        AddParameter(command, "@bairro", entity.Bairro);
        AddParameter(command, "@cidade", entity.Cidade);
        AddParameter(command, "@estado", entity.Estado);
        AddParameter(command, "@pais", entity.Pais);

        int rows = await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
        if (rows == 0)
            throw new InfrastructureException($"Registro com ID {entity.Id} não encontrado para atualização.");

        return entity;
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        const string query = "DELETE FROM tb_logradouro WHERE id_logradouro = @id;";

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@id", id);

        int rows = await Task.Run(() => command.ExecuteNonQuery(), cancellationToken);
        return rows > 0;
    }

    public async Task<Logradouro?> ObterPorCep(Cep cep, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT id_logradouro, cep, nome, bairro, cidade, estado, pais FROM tb_logradouro WHERE cep = @cep;";

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@cep", cep.Valor);

        using var reader = (IDataReader)await Task.Run(() => command.ExecuteReader(), cancellationToken);
        if (reader.Read())
        {
            return MapToDomain(reader);
        }

        return null;
    }

    public async Task<bool> CepJaExiste(Cep cep, int? id = null, CancellationToken cancellationToken = default)
    {
        string query = id.HasValue
            ? "SELECT COUNT(1) FROM tb_logradouro WHERE cep = @cep AND id_logradouro <> @id;"
            : "SELECT COUNT(1) FROM tb_logradouro WHERE cep = @cep;";

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@cep", cep.Valor);
        if (id.HasValue)
        {
            AddParameter(command, "@id", id.Value);
        }

        object? count = await Task.Run(() => command.ExecuteScalar(), cancellationToken);
        return Convert.ToInt32(count) > 0;
    }

    public async Task<IEnumerable<Logradouro>> ObterPorCidade(string cidade, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT id_logradouro, cep, nome, bairro, cidade, estado, pais FROM tb_logradouro WHERE cidade = @cidade ORDER BY nome;";
        var list = new List<Logradouro>();

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@cidade", cidade);

        using var reader = (IDataReader)await Task.Run(() => command.ExecuteReader(), cancellationToken);
        while (reader.Read())
        {
            list.Add(MapToDomain(reader));
        }

        return list;
    }

    public async Task<IEnumerable<Logradouro>> ObterPorBairro(string cidade, string bairro, CancellationToken cancellationToken = default)
    {
        const string query = "SELECT id_logradouro, cep, nome, bairro, cidade, estado, pais FROM tb_logradouro WHERE cidade = @cidade AND bairro = @bairro ORDER BY nome;";
        var list = new List<Logradouro>();

        using var connection = CreateConnection();
        connection.Open();

        using var command = CreateCommand(query, connection);
        AddParameter(command, "@cidade", cidade);
        AddParameter(command, "@bairro", bairro);

        using var reader = (IDataReader)await Task.Run(() => command.ExecuteReader(), cancellationToken);
        while (reader.Read())
        {
            list.Add(MapToDomain(reader));
        }

        return list;
    }

    private static Logradouro MapToDomain(IDataRecord reader)
    {
        int id = Convert.ToInt32(reader["id_logradouro"]);
        string cep = Convert.ToString(reader["cep"]) ?? string.Empty;
        string nome = Convert.ToString(reader["nome"]) ?? string.Empty;
        string bairro = Convert.ToString(reader["bairro"]) ?? string.Empty;
        string cidade = Convert.ToString(reader["cidade"]) ?? string.Empty;
        string estado = Convert.ToString(reader["estado"]) ?? string.Empty;
        string pais = Convert.ToString(reader["pais"]) ?? string.Empty;

        var result = Logradouro.Criar(id, cep, nome, bairro, cidade, estado, pais);
        if (result.IsFailure)
            throw new InfrastructureException($"Falha ao reconstituir entidade Logradouro com ID {id}.");

        return result.Value!;
    }
    public static Logradouro Map(System.Data.Common.DbDataReader reader, string nomeColumn = "nome")
    {
        int id = reader.GetInt32Value("id_logradouro");
        string cep = reader.GetStringValue("cep");
        string nome = reader.GetStringValue(nomeColumn);
        string bairro = reader.GetStringValue("bairro");
        string cidade = reader.GetStringValue("cidade");
        string estado = reader.GetStringValue("estado");
        string pais = reader.GetStringValue("pais");

        var result = Logradouro.Criar(id, cep, nome, bairro, cidade, estado, pais);
        if (result.IsFailure)
            throw new InfrastructureException("ERRO_MAPEAMENTO_LOGRADOURO", $"Erro ao mapear logradouro ID {id}: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }
}