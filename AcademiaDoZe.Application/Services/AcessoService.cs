using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
// Pedro Henrique dos Santos

namespace AcademiaDoZe.Application.Services;

public class AcessoService : IAcessoService
{
    private readonly Func<IAcessoAlunoRepository> _acessoAlunoRepoFactory;
    private readonly Func<IAcessoColaboradorRepository> _acessoColaboradorRepoFactory;
    private readonly Func<IAlunoRepository> _alunoRepoFactory;
    private readonly Func<IColaboradorRepository> _colaboradorRepoFactory;
    private readonly Func<IMatriculaRepository> _matriculaRepoFactory;
    private readonly RepositoryConfig _config;

    // Bancos (tipo + string de conexão) que já tiveram as tabelas de acesso garantidas nesta execução.
    private static readonly HashSet<string> _bancosInicializados = [];
    private static readonly SemaphoreSlim _initLock = new(1, 1);

    public AcessoService(
        Func<IAcessoAlunoRepository> acessoAlunoRepoFactory,
        Func<IAcessoColaboradorRepository> acessoColaboradorRepoFactory,
        Func<IAlunoRepository> alunoRepoFactory,
        Func<IColaboradorRepository> colaboradorRepoFactory,
        Func<IMatriculaRepository> matriculaRepoFactory,
        RepositoryConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _acessoAlunoRepoFactory = acessoAlunoRepoFactory ?? throw new ArgumentNullException(nameof(acessoAlunoRepoFactory));
        _acessoColaboradorRepoFactory = acessoColaboradorRepoFactory ?? throw new ArgumentNullException(nameof(acessoColaboradorRepoFactory));
        _alunoRepoFactory = alunoRepoFactory ?? throw new ArgumentNullException(nameof(alunoRepoFactory));
        _colaboradorRepoFactory = colaboradorRepoFactory ?? throw new ArgumentNullException(nameof(colaboradorRepoFactory));
        _matriculaRepoFactory = matriculaRepoFactory ?? throw new ArgumentNullException(nameof(matriculaRepoFactory));
    }

    /// <summary>
    /// Garante que as tabelas de acesso existam no banco ativo. Os scripts usam CREATE TABLE IF NOT EXISTS,
    /// então rodar de novo é inofensivo; o cache evita repetir a cada chamada.
    /// </summary>
    private async Task GarantirEsquemaAsync(CancellationToken cancellationToken)
    {
        string chave = $"{_config.DatabaseType}|{_config.ConnectionString}";
        await _initLock.WaitAsync(cancellationToken);
        try
        {
            if (_bancosInicializados.Contains(chave)) return;
            await DbInitializer.InitializeAsync(_config.DatabaseType, _config.ConnectionString, cancellationToken);
            _bancosInicializados.Add(chave);
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<bool> AlunoEstaNaAcademiaAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        await GarantirEsquemaAsync(cancellationToken);
        return await _acessoAlunoRepoFactory().EstaNaAcademia(alunoId, cancellationToken);
    }

    public async Task<bool> ColaboradorEstaNaAcademiaAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        await GarantirEsquemaAsync(cancellationToken);
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var acessos = await _acessoColaboradorRepoFactory().ObterAcessosPorColaboradorPeriodo(colaboradorId, hoje, hoje, cancellationToken);
        return acessos.Count() % 2 == 1;
    }

    public async Task<AcessoResultadoDto> RegistrarAcessoAlunoAsync(int alunoId, CancellationToken cancellationToken = default)
    {
        await GarantirEsquemaAsync(cancellationToken);
        var aluno = await _alunoRepoFactory().ObterPorId(alunoId, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

        var agora = DateTime.Now;
        var repo = _acessoAlunoRepoFactory();
        var hoje = DateOnly.FromDateTime(agora);
        var acessosHoje = (await repo.ObterAcessosPorAlunoPeriodo(alunoId, hoje, hoje, cancellationToken)).ToList();
        bool estaDentro = acessosHoje.Count % 2 == 1;

        if (!estaDentro)
        {
            // Regra: só entra quem tem matrícula ativa.
            var matricula = await _matriculaRepoFactory().ObterMatriculaAtivaPorAluno(alunoId, cancellationToken);
            if (matricula == null)
                return Negado("Acesso Negado", "Acesso negado: o aluno não possui matrícula ativa.", agora);

            var registro = AcessoAluno.Criar(0, aluno, agora);
            if (registro.IsFailure)
                return Negado("Acesso Negado", TraduzirErro(registro.Notifications), agora);

            await repo.Adicionar(registro.Value!, cancellationToken);

            // Regra: na entrada, mostrar quanto tempo ainda tem de plano.
            int diasRestantes = Math.Max(0, matricula.DataFim.DayNumber - hoje.DayNumber);
            return new AcessoResultadoDto
            {
                Autorizado = true,
                EhEntrada = true,
                Titulo = "Entrada Autorizada",
                Mensagem = $"Entrada autorizada. Dias restantes de plano: {diasRestantes} dia(s).",
                DataHora = agora
            };
        }

        var saida = AcessoAluno.Criar(0, aluno, agora);
        if (saida.IsFailure)
            return Negado("Saída Negada", TraduzirErro(saida.Notifications), agora);

        await repo.Adicionar(saida.Value!, cancellationToken);

        // Regra: na saída, mostrar o tempo que permaneceu na academia.
        var permanencia = agora - acessosHoje[^1].DataHora;
        return new AcessoResultadoDto
        {
            Autorizado = true,
            EhEntrada = false,
            Titulo = "Saída Registrada",
            Mensagem = $"Saída registrada. Tempo de permanência: {Formatar(permanencia)}.",
            DataHora = agora
        };
    }

    public async Task<AcessoResultadoDto> RegistrarAcessoColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default)
    {
        await GarantirEsquemaAsync(cancellationToken);
        var colaborador = await _colaboradorRepoFactory().ObterPorId(colaboradorId, cancellationToken)
            ?? throw new InvalidOperationException($"Colaborador com ID {colaboradorId} não encontrado.");

        var agora = DateTime.Now;
        var repo = _acessoColaboradorRepoFactory();
        var hoje = DateOnly.FromDateTime(agora);
        var acessosHoje = (await repo.ObterAcessosPorColaboradorPeriodo(colaboradorId, hoje, hoje, cancellationToken)).ToList();
        bool estaDentro = acessosHoje.Count % 2 == 1;

        var registro = AcessoColaborador.Criar(0, colaborador, agora);
        if (registro.IsFailure)
            return Negado(estaDentro ? "Saída Negada" : "Acesso Negado", TraduzirErro(registro.Notifications), agora);

        if (!estaDentro)
        {
            // Regra: não pode entrar se já completou o limite diário (8h CLT, 6h estágio).
            var limite = colaborador.Vinculo == ColaboradorVinculo.CLT ? TimeSpan.FromHours(8) : TimeSpan.FromHours(6);
            var trabalhadas = await repo.ObterHorasTrabalhadasNoDia(colaboradorId, hoje, cancellationToken);
            if (trabalhadas >= limite)
                return Negado("Acesso Negado",
                    $"Acesso negado: limite diário de {(int)limite.TotalHours}h ({colaborador.Vinculo}) já atingido. Horas trabalhadas hoje: {Formatar(trabalhadas)}.", agora);

            await repo.Adicionar(registro.Value!, cancellationToken);
            return new AcessoResultadoDto
            {
                Autorizado = true,
                EhEntrada = true,
                Titulo = "Entrada Registrada",
                Mensagem = $"Entrada registrada. Horas trabalhadas hoje até o momento: {Formatar(trabalhadas)}.",
                DataHora = agora
            };
        }

        await repo.Adicionar(registro.Value!, cancellationToken);

        // Regra: na saída, mostrar o tempo de permanência somando todos os registros do dia.
        var total = await repo.ObterHorasTrabalhadasNoDia(colaboradorId, hoje, cancellationToken);
        return new AcessoResultadoDto
        {
            Autorizado = true,
            EhEntrada = false,
            Titulo = "Saída Registrada",
            Mensagem = $"Saída registrada. Tempo de permanência hoje (soma dos registros): {Formatar(total)}.",
            DataHora = agora
        };
    }

    public async Task<IEnumerable<AcessoDto>> ObterAcessosDeHojeAsync(CancellationToken cancellationToken = default)
    {
        await GarantirEsquemaAsync(cancellationToken);
        var hoje = DateOnly.FromDateTime(DateTime.Now);
        var itens = new List<AcessoDto>();

        var alunos = await _acessoAlunoRepoFactory().ObterAcessosPorAlunoPeriodo(null, hoje, hoje, cancellationToken);
        foreach (var grupo in alunos.GroupBy(a => a.AlunoId))
        {
            var ordenados = grupo.OrderBy(a => a.DataHora).ThenBy(a => a.Id).ToList();
            var aluno = await _alunoRepoFactory().ObterPorId(grupo.Key, cancellationToken);
            for (int i = 0; i < ordenados.Count; i++)
            {
                bool entrada = i % 2 == 0;
                itens.Add(new AcessoDto
                {
                    Id = ordenados[i].Id,
                    PessoaId = grupo.Key,
                    Nome = aluno?.Nome ?? $"Aluno {grupo.Key}",
                    TipoPessoa = "Aluno",
                    Tipo = entrada ? "Entrada" : "Saída",
                    DataHora = ordenados[i].DataHora,
                    Descricao = entrada ? "Entrada registrada." : $"Saída registrada. Tempo de permanência: {Formatar(ordenados[i].DataHora - ordenados[i - 1].DataHora)}.",
                    Foto = aluno?.Foto?.Conteudo != null ? new ArquivoDto { Conteudo = aluno.Foto.Conteudo } : null
                });
            }
        }

        var colaboradores = await _acessoColaboradorRepoFactory().ObterAcessosPorColaboradorPeriodo(null, hoje, hoje, cancellationToken);
        foreach (var grupo in colaboradores.GroupBy(a => a.ColaboradorId))
        {
            var ordenados = grupo.OrderBy(a => a.DataHora).ThenBy(a => a.Id).ToList();
            var colaborador = await _colaboradorRepoFactory().ObterPorId(grupo.Key, cancellationToken);
            var acumulado = TimeSpan.Zero;
            for (int i = 0; i < ordenados.Count; i++)
            {
                bool entrada = i % 2 == 0;
                if (!entrada) acumulado += ordenados[i].DataHora - ordenados[i - 1].DataHora;
                itens.Add(new AcessoDto
                {
                    Id = ordenados[i].Id,
                    PessoaId = grupo.Key,
                    Nome = colaborador?.Nome ?? $"Colaborador {grupo.Key}",
                    TipoPessoa = "Colaborador",
                    Tipo = entrada ? "Entrada" : "Saída",
                    DataHora = ordenados[i].DataHora,
                    Descricao = entrada
                        ? $"Entrada registrada. Horas trabalhadas hoje até o momento: {Formatar(acumulado)}."
                        : $"Saída registrada. Tempo de permanência hoje: {Formatar(acumulado)}.",
                    Foto = colaborador?.Foto?.Conteudo != null ? new ArquivoDto { Conteudo = colaborador.Foto.Conteudo } : null
                });
            }
        }

        return [.. itens.OrderByDescending(i => i.DataHora)];
    }

    private static AcessoResultadoDto Negado(string titulo, string mensagem, DateTime agora) => new()
    {
        Autorizado = false,
        Titulo = titulo,
        Mensagem = mensagem,
        DataHora = agora
    };

    private static string TraduzirErro(IEnumerable<AcademiaDoZe.Domain.Common.Notification> notificacoes)
    {
        var codigos = notificacoes.Select(n => n.Mensagem).ToList();
        return codigos.Contains("DATA_HORA_INTERVALO_INVALIDO")
            ? "Acesso negado: a academia só funciona entre 06:00 e 22:00."
            : $"Acesso negado: {string.Join(", ", codigos)}.";
    }

    private static string Formatar(TimeSpan tempo) => $"{(int)tempo.TotalHours:00}:{tempo.Minutes:00}:{tempo.Seconds:00}";
}
