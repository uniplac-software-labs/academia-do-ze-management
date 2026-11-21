using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;
// Pedro Henrique dos Santos

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoAlunoRepository : BaseRepository, IAcessoAlunoRepository
{
    public AcessoAlunoRepository(string connectionString, DatabaseType databaseType) : base(connectionString, databaseType) { }

    private static string BaseSelectQuery => """
        SELECT
        ac.id_acesso_aluno, ac.data_hora,
        a.id_aluno, a.cpf, a.nome AS aluno_nome, a.nascimento, a.telefone, a.email, a.logradouro_id, a.numero, a.complemento, a.senha, a.foto,
        l.id_logradouro, l.cep, l.nome AS logradouro_nome, l.bairro, l.cidade, l.estado, l.pais
        FROM tb_acesso_aluno ac
        INNER JOIN tb_aluno a ON ac.aluno_id = a.id_aluno
        INNER JOIN tb_logradouro l ON a.logradouro_id = l.id_logradouro
        """;

    public static AcessoAluno Map(DbDataReader reader)
    {
        try
        {
            int id = reader.GetInt32Value("id_acesso_aluno");
            DateTime dataHora = reader.GetDateTimeValue("data_hora");
            var aluno = AlunoRepository.Map(reader, "aluno_nome");

            var result = AcessoAluno.Criar(id, aluno, dataHora);
            if (result.IsFailure)
                throw new InfrastructureException("ERRO_DOMINIO_MAPEAMENTO", $"Erro de domínio ao mapear acesso de aluno ID {id}: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

            return result.Value!;
        }
        catch (Exception ex) when (ex is not InfrastructureException)
        {
            throw new InfrastructureException("ERRO_MAPEAMENTO_ACESSO_ALUNO", $"Erro ao mapear dados do acesso do aluno: {ex.Message}", ex);
        }
    }

    private async Task<List<AcessoAluno>> ExecutarListaAsync(string query, Action<DbCommand>? parametros, string codigoErro, string mensagemErro, CancellationToken cancellationToken)
    {
        try
        {
            await using var command = await CreateCommandAsync(query, cancellationToken);
            parametros?.Invoke(command);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var lista = new List<AcessoAluno>();
            while (await reader.ReadAsync(cancellationToken)) lista.Add(Map(reader));
            return lista;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(codigoErro, $"{mensagemErro}: {ex.Message}", ex);
        }
    }

    public async Task<AcessoAluno?> ObterPorId(int id, CancellationToken cancellationToken = default)
    {
        var lista = await ExecutarListaAsync($"{BaseSelectQuery} WHERE ac.id_acesso_aluno = @Id",
            c => c.AddParameter("@Id", id, DbType.Int32), "ERRO_OBTER_POR_ID", $"Erro ao obter acesso de aluno por ID {id}", cancellationToken);
        return lista.FirstOrDefault();
    }

    public async Task<IEnumerable<AcessoAluno>> ObterTodos(CancellationToken cancellationToken = default)
        => await ExecutarListaAsync($"{BaseSelectQuery} ORDER BY ac.data_hora DESC, ac.id_acesso_aluno DESC", null,
            "ERRO_OBTER_TODOS", "Erro ao obter todos os acessos de alunos", cancellationToken);

    public async Task<AcessoAluno> Adicionar(AcessoAluno entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = FormatInsertQuery("INSERT INTO tb_acesso_aluno (aluno_id, data_hora) VALUES (@AlunoId, @DataHora)");
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@AlunoId", entity.AlunoId, DbType.Int32);
            command.AddParameter("@DataHora", AcessoCalculos.Formatar(entity.DataHora), DbType.String);
            int id = await command.ExecuteScalarIdAsync("ERRO_ADICIONAR_ACESSO_ALUNO", "Falha ao obter ID inserido para o acesso do aluno.", cancellationToken);
            typeof(Entity).GetProperty("Id")?.SetValue(entity, id);
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ADICIONAR_ACESSO_ALUNO", $"Erro ao adicionar acesso para o aluno ID {entity.AlunoId}: {ex.Message}", ex);
        }
    }

    public async Task<AcessoAluno> Atualizar(AcessoAluno entity, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = "UPDATE tb_acesso_aluno SET aluno_id = @AlunoId, data_hora = @DataHora WHERE id_acesso_aluno = @Id";
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Id", entity.Id, DbType.Int32);
            command.AddParameter("@AlunoId", entity.AlunoId, DbType.Int32);
            command.AddParameter("@DataHora", AcessoCalculos.Formatar(entity.DataHora), DbType.String);
            int rows = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rows == 0)
                throw new InfrastructureException("REGISTRO_NAO_ENCONTRADO", $"Nenhum acesso encontrado com o ID {entity.Id} para atualização.");
            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ATUALIZAR_ACESSO_ALUNO", $"Erro ao atualizar acesso ID {entity.Id}: {ex.Message}", ex);
        }
    }

    public async Task<bool> Remover(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var command = await CreateCommandAsync("DELETE FROM tb_acesso_aluno WHERE id_acesso_aluno = @Id", cancellationToken);
            command.AddParameter("@Id", id, DbType.Int32);
            return await command.ExecuteNonQueryAsync(cancellationToken) > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_REMOVER_ACESSO_ALUNO", $"Erro ao remover acesso ID {id}: {ex.Message}", ex);
        }
    }

    public async Task<IEnumerable<AcessoAluno>> ObterAcessosPorAlunoPeriodo(int? alunoId = null, DateOnly? inicio = null, DateOnly? fim = null, CancellationToken cancellationToken = default)
    {
        var filtros = new List<string>();
        if (alunoId.HasValue) filtros.Add("ac.aluno_id = @AlunoId");
        if (inicio.HasValue) filtros.Add("ac.data_hora >= @Inicio");
        if (fim.HasValue) filtros.Add("ac.data_hora < @FimExclusivo");
        string where = filtros.Count > 0 ? "WHERE " + string.Join(" AND ", filtros) : string.Empty;

        return await ExecutarListaAsync($"{BaseSelectQuery} {where} ORDER BY ac.data_hora ASC, ac.id_acesso_aluno ASC", c =>
        {
            if (alunoId.HasValue) c.AddParameter("@AlunoId", alunoId.Value, DbType.Int32);
            if (inicio.HasValue) c.AddParameter("@Inicio", AcessoCalculos.Formatar(inicio.Value.ToDateTime(TimeOnly.MinValue)), DbType.String);
            if (fim.HasValue) c.AddParameter("@FimExclusivo", AcessoCalculos.Formatar(fim.Value.AddDays(1).ToDateTime(TimeOnly.MinValue)), DbType.String);
        }, "ERRO_OBTER_ACESSOS_PERIODO", "Erro ao obter acessos de alunos por período", cancellationToken);
    }

    public async Task<AcessoAluno?> ObterUltimoAcesso(int alunoId, CancellationToken cancellationToken = default)
    {
        var lista = await ExecutarListaAsync($"{BaseSelectQuery} WHERE ac.aluno_id = @AlunoId ORDER BY ac.data_hora DESC, ac.id_acesso_aluno DESC",
            c => c.AddParameter("@AlunoId", alunoId, DbType.Int32), "ERRO_OBTER_ULTIMO_ACESSO", $"Erro ao obter último acesso do aluno ID {alunoId}", cancellationToken);
        return lista.FirstOrDefault();
    }

    /// <summary>O aluno está dentro quando possui uma quantidade ímpar de registros hoje (entrou e ainda não saiu).</summary>
    public async Task<bool> EstaNaAcademia(int alunoId, CancellationToken cancellationToken = default)
    {
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var acessos = await ObterAcessosPorAlunoPeriodo(alunoId, hoje, hoje, cancellationToken);
        return acessos.Count() % 2 == 1;
    }

    public async Task<Dictionary<TimeOnly, int>> ObterHorarioMaisProcuradoPorMes(int mes, CancellationToken cancellationToken = default)
    {
        var entradas = ObterEntradas(await ObterAcessosDoMes(mes, cancellationToken));
        return entradas
            .GroupBy(e => new TimeOnly(e.Entrada.Hour, 0))
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<int, TimeSpan>> ObterPermanenciaMediaPorMes(int mes, CancellationToken cancellationToken = default)
    {
        var porAlunoDia = (await ObterAcessosDoMes(mes, cancellationToken))
            .GroupBy(a => (a.AlunoId, Dia: a.DataHora.Date));

        var permanencias = new Dictionary<int, List<TimeSpan>>();
        foreach (var grupo in porAlunoDia)
        {
            var horarios = grupo.Select(a => a.DataHora).OrderBy(h => h).ToList();
            for (int i = 0; i + 1 < horarios.Count; i += 2)
            {
                if (!permanencias.TryGetValue(grupo.Key.AlunoId, out var lista))
                    permanencias[grupo.Key.AlunoId] = lista = [];
                lista.Add(horarios[i + 1] - horarios[i]);
            }
        }

        return permanencias.ToDictionary(p => p.Key, p => TimeSpan.FromTicks((long)p.Value.Average(t => t.Ticks)));
    }

    public async Task<IEnumerable<Aluno>> ObterAlunosSemAcessoNosUltimosDias(int dias, CancellationToken cancellationToken = default)
    {
        try
        {
            string query = """
                SELECT
                a.id_aluno, a.cpf, a.nome, a.nascimento, a.telefone, a.email, a.logradouro_id, a.numero, a.complemento, a.senha, a.foto,
                l.id_logradouro, l.cep, l.nome AS logradouro_nome, l.bairro, l.cidade, l.estado, l.pais
                FROM tb_aluno a
                INNER JOIN tb_logradouro l ON a.logradouro_id = l.id_logradouro
                WHERE NOT EXISTS (SELECT 1 FROM tb_acesso_aluno ac WHERE ac.aluno_id = a.id_aluno AND ac.data_hora >= @Limite)
                ORDER BY a.nome
                """;
            await using var command = await CreateCommandAsync(query, cancellationToken);
            command.AddParameter("@Limite", AcessoCalculos.Formatar(DateTime.Today.AddDays(-dias)), DbType.String);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var alunos = new List<Aluno>();
            while (await reader.ReadAsync(cancellationToken)) alunos.Add(AlunoRepository.Map(reader));
            return alunos;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException("ERRO_ALUNOS_SEM_ACESSO", $"Erro ao obter alunos sem acesso nos últimos {dias} dias: {ex.Message}", ex);
        }
    }

    private async Task<List<AcessoAluno>> ObterAcessosDoMes(int mes, CancellationToken cancellationToken)
    {
        if (mes is < 1 or > 12) throw new ArgumentOutOfRangeException(nameof(mes), "Mês deve estar entre 1 e 12.");
        var primeiro = new DateOnly(DateTime.Today.Year, mes, 1);
        var ultimo = primeiro.AddMonths(1).AddDays(-1);
        return [.. await ObterAcessosPorAlunoPeriodo(null, primeiro, ultimo, cancellationToken)];
    }

    // Considera como "entrada" o 1º, 3º, 5º... registro de cada aluno em cada dia.
    private static IEnumerable<(int AlunoId, DateTime Entrada)> ObterEntradas(IEnumerable<AcessoAluno> acessos)
    {
        foreach (var grupo in acessos.GroupBy(a => (a.AlunoId, Dia: a.DataHora.Date)))
        {
            var horarios = grupo.Select(a => a.DataHora).OrderBy(h => h).ToList();
            for (int i = 0; i < horarios.Count; i += 2)
                yield return (grupo.Key.AlunoId, horarios[i]);
        }
    }
}
