using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;
// Pedro Henrique dos Santos

namespace AcademiaDoZe.Infrastructure.Tests;

public class AcessoInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;
    private readonly ColaboradorRepository _colaboradorRepo;
    private readonly AcessoAlunoRepository _acessoAlunoRepo;
    private readonly AcessoColaboradorRepository _acessoColaboradorRepo;

    public AcessoInfrastructureTests()
    {
        _logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
        _alunoRepo = new AlunoRepository(ConnectionString, DatabaseType);
        _colaboradorRepo = new ColaboradorRepository(ConnectionString, DatabaseType);
        _acessoAlunoRepo = new AcessoAlunoRepository(ConnectionString, DatabaseType);
        _acessoColaboradorRepo = new AcessoColaboradorRepository(ConnectionString, DatabaseType);
    }

    private static DateTime Hoje(int hora, int minuto = 0) => DateTime.Today.AddHours(hora).AddMinutes(minuto);

    [Fact]
    public async Task AcessoAluno_Adicionar_ObterUltimo_E_EstaNaAcademia()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);

        Assert.False(await _acessoAlunoRepo.EstaNaAcademia(aluno.Id));

        var entrada = await _acessoAlunoRepo.Adicionar(AcessoAluno.Criar(0, aluno, Hoje(8)).Value!);
        Assert.True(entrada.Id > 0);
        Assert.True(await _acessoAlunoRepo.EstaNaAcademia(aluno.Id));

        await _acessoAlunoRepo.Adicionar(AcessoAluno.Criar(0, aluno, Hoje(9, 30)).Value!);
        Assert.False(await _acessoAlunoRepo.EstaNaAcademia(aluno.Id));

        var ultimo = await _acessoAlunoRepo.ObterUltimoAcesso(aluno.Id);
        Assert.NotNull(ultimo);
        Assert.Equal(Hoje(9, 30), ultimo.DataHora);
    }

    [Fact]
    public async Task AcessoAluno_ObterPorPeriodo_FiltraPorAluno()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var outro = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        await _acessoAlunoRepo.Adicionar(AcessoAluno.Criar(0, aluno, Hoje(7)).Value!);
        await _acessoAlunoRepo.Adicionar(AcessoAluno.Criar(0, outro, Hoje(7, 5)).Value!);

        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var acessos = (await _acessoAlunoRepo.ObterAcessosPorAlunoPeriodo(aluno.Id, hoje, hoje)).ToList();

        Assert.Single(acessos);
        Assert.Equal(aluno.Id, acessos[0].AlunoId);
    }

    [Fact]
    public async Task AcessoAluno_RemoverPorId_Sucesso()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var acesso = await _acessoAlunoRepo.Adicionar(AcessoAluno.Criar(0, aluno, Hoje(10)).Value!);

        Assert.True(await _acessoAlunoRepo.Remover(acesso.Id));
        Assert.Null(await _acessoAlunoRepo.ObterPorId(acesso.Id));
    }

    [Fact]
    public async Task AcessoColaborador_SomaApenasParesFechadosDoDia()
    {
        var colaborador = await ColaboradorInfrastructureTests.CriarEInserirColaboradorAsync(_colaboradorRepo, _logradouroRepo);

        await _acessoColaboradorRepo.Adicionar(AcessoColaborador.Criar(0, colaborador, Hoje(8)).Value!);
        await _acessoColaboradorRepo.Adicionar(AcessoColaborador.Criar(0, colaborador, Hoje(12)).Value!);
        await _acessoColaboradorRepo.Adicionar(AcessoColaborador.Criar(0, colaborador, Hoje(13)).Value!);
        await _acessoColaboradorRepo.Adicionar(AcessoColaborador.Criar(0, colaborador, Hoje(15, 30)).Value!);
        // entrada ainda aberta: não deve entrar na soma
        await _acessoColaboradorRepo.Adicionar(AcessoColaborador.Criar(0, colaborador, Hoje(16)).Value!);

        var horas = await _acessoColaboradorRepo.ObterHorasTrabalhadasNoDia(colaborador.Id, DateOnly.FromDateTime(DateTime.Today));

        Assert.Equal(TimeSpan.FromHours(6.5), horas);
    }
}
