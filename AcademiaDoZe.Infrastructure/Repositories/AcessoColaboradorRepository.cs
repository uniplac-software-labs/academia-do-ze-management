using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;
// Pedro Henrique dos Santos

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoColaboradorRepository : BaseRepository, IAcessoColaboradorRepository
{
    public AcessoColaboradorRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

    private static string BaseSelectQuery => """
        SELECT
        ac.id_acesso_colaborador, ac.data_hora,
        c.id_colaborador, c.cpf, c.nome AS colaborador_nome, c.nascimento, c.telefone, c.email, c.logradouro_id, c.numero, c.complemento, c.senha, c.foto, c.admissao, c.tipo, c.vinculo,
        l.id_logradouro, l.cep, l.nome AS logradouro_nome, l.bairro, l.cidade, l.estado, l.pais
        FROM tb_acesso_colaborador ac
        INNER JOIN tb_colaborador c ON ac.colaborador_id = c.id_colaborador
        INNER JOIN tb_logradouro l ON c.logradouro_id = l.id_logradouro
        """;

    public static AcessoColaborador Map(DbDataReader reader)
    {
        try
        {
            int id = reader.GetInt32Value("id_acesso_colaborador");
            DateTime dataHora = reader.GetDateTimeValue("data_hora");
            var colaborador = ColaboradorRepository.Map(reader, "colaborador_nome");

            var result = AcessoColaborador.Criar(id, colaborador, dataHora);
            if (result.IsFailure)
                throw new InfrastructureException("ERRO_DOMINIO_MAPEAMENTO", $"Erro de domínio ao mapear acesso de colaborador ID {id}: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

            return result.Value!;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("ERRO_MAPEAMENTO_ACESSO_COLABORADOR", $"Erro ao mapear dados do acesso do colaborador: {ex.Message}", ex);
        }
    }

    private async Task<List<AcessoColaborador>> ExecutarListaAsync(string query, Action<DbCommand>? parametros, string codigoErro, string mensagemErro, CancellationToken cancellationToken)
    {
        try
        {
            await using var command = await CreateCommandAsync(query, cancellationToken);
            parametros?.Invoke(command);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var lista = new List<AcessoColaborador>();
            while (await reader.ReadAsync(cancellationToken)) lista.Add(Map(reader));
            return lista;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(codigoErro, $"{mensagemErro}: {ex.Message}", ex);
        }
    }

    public async Task<AcessoColaborador?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        var lista = await ExecutarListaAsync($"{BaseSelectQuery} WHERE ac.id_acesso_colaborador = @Id",
            c => c.AddParameter("@Id", id, DbType.Int32), "ERRO_OBTER_POR_ID", $"Erro ao obter acesso de colaborador por ID {id}", cancellationToken);
        return lista.FirstOrDefault();
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterTodos(CancellationToken cancellationToken = default)
        => await ExecutarListaAsync($"{BaseSelectQuery} ORDER BY ac.data_hora DESC, ac.id_acesso_colaborador DESC", null,
            "ERRO_OBTER_TODOS", "Erro ao obter todos os acessos de colaboradores", cancellationToken);

    public async Task<AcessoColaborador> Adicionar(AcessoColaborador entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery("INSERT INTO tb_acesso_colaborador (colaborador_id, data_hora) VALUES (@ColaboradorId, @DataHora)");
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@ColaboradorId", entity.ColaboradorId, DbType.Int32);
            command.AddParameter("@DataHora", AcessoCalculos.Formatar(entity.DataHora), DbType.String);
            int id = await command.ExecuteScalarIdAsync("ERRO_ADICIONAR_ACESSO_COLABORADOR", "Falha ao obter ID inserido para o acesso do colaborador.", cancellationToken);
            typeof(Entity).GetProperty("Id")?.SetValue(entity, id);
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_ACESSO_COLABORADOR", $"Erro ao adicionar acesso para o colaborador ID {entity.ColaboradorId}: {ex.Message}", ex);
        }
    }

    public async Task<AcessoColaborador> Atualizar(AcessoColaborador entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = "UPDATE tb_acesso_colaborador SET colaborador_id = @ColaboradorId, data_hora = @DataHora WHERE id_acesso_colaborador = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", entity.Id, DbType.Int32);
            command.AddParameter("@ColaboradorId", entity.ColaboradorId, DbType.Int32);
            command.AddParameter("@DataHora", AcessoCalculos.Formatar(entity.DataHora), DbType.String);
            int rows = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rows == 0)
                throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO", $"Nenhum acesso encontrado com o ID {entity.Id} para atualização.");
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_ACESSO_COLABORADOR", $"Erro ao atualizar acesso ID {entity.Id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = await CreateCommandAsync("DELETE FROM tb_acesso_colaborador WHERE id_acesso_colaborador = @Id", cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_REMOVER_ACESSO_COLABORADOR", $"Erro ao remover acesso ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterAcessosPorColaboradorPeriodo(int? colaboradorId = null, DateOnly? inicio = null, DateOnly? fim = null, CancellationToken cancellationToken = default)
    {
        var filtros = new List<string>();
        if (colaboradorId.HasValue) filtros.Add("ac.colaborador_id = @ColaboradorId");
        if (inicio.HasValue) filtros.Add("ac.data_hora >= @Inicio");
        if (fim.HasValue) filtros.Add("ac.data_hora < @FimExclusivo");
        string where = filtros.Count > 0 ? "WHERE " + string.Join(" AND ", filtros) : string.Empty;

        return await ExecutarListaAsync($"{BaseSelectQuery} {where} ORDER BY ac.data_hora ASC, ac.id_acesso_colaborador ASC", c =>
        {
            if (colaboradorId.HasValue) c.AddParameter("@ColaboradorId", colaboradorId.Value, DbType.Int32);
            if (inicio.HasValue) c.AddParameter("@Inicio", AcessoCalculos.Formatar(inicio.Value.ToDateTime(TimeOnly.MinValue)), DbType.String);
            if (fim.HasValue) c.AddParameter("@FimExclusivo", AcessoCalculos.Formatar(fim.Value.AddDays(1).ToDateTime(TimeOnly.MinValue)), DbType.String);
        }, "ERRO_OBTER_ACESSOS_PERIODO", "Erro ao obter acessos de colaboradores por período", cancellationToken);
    }

    public async Task<AcessoColaborador?> ObterUltimoAcesso(int colaboradorId, CancellationToken cancellationToken = default)
    {
        var lista = await ExecutarListaAsync($"{BaseSelectQuery} WHERE ac.colaborador_id = @ColaboradorId ORDER BY ac.data_hora DESC, ac.id_acesso_colaborador DESC",
            c => c.AddParameter("@ColaboradorId", colaboradorId, DbType.Int32), "ERRO_OBTER_ULTIMO_ACESSO", $"Erro ao obter último acesso do colaborador ID {colaboradorId}", cancellationToken);
        return lista.FirstOrDefault();
    }

    /// <summary>Soma os pares fechados (entrada + saída) do dia; uma entrada ainda aberta não entra na soma.</summary>
    public async Task<TimeSpan> ObterHorasTrabalhadasNoDia(int colaboradorId, DateOnly data, CancellationToken cancellationToken = default)
    {
        var acessos = await ObterAcessosPorColaboradorPeriodo(colaboradorId, data, data, cancellationToken);
        return AcessoCalculos.SomarPares([.. acessos.Select(a => a.DataHora).OrderBy(h => h)]);
    }
}
