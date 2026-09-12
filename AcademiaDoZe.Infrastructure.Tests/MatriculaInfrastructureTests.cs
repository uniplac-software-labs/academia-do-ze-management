using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class MatriculaInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;
    private readonly MatriculaRepository _matriculaRepo;

    public MatriculaInfrastructureTests()
    {
        _logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
        _alunoRepo = new AlunoRepository(ConnectionString, DatabaseType);
        _matriculaRepo = new MatriculaRepository(ConnectionString, DatabaseType);
    }

    // Pedro Henrqiue dos Santos
    private async Task<Matricula> CriarEInserirMatriculaAsync(
        Aluno aluno,
        MatriculaPlano plano = MatriculaPlano.Mensal,
        DateOnly? dataInicio = null,
        MatriculaRestricoes restricoes = MatriculaRestricoes.None,
        string? obsRestricao = null,
        Arquivo? laudo = null)
    {
        var inicio = dataInicio ?? DateOnly.FromDateTime(DateTime.Today);
        if (restricoes != MatriculaRestricoes.None && laudo == null)
            laudo = Arquivo.Criar(new byte[] { 1, 2, 3, 4 }).Value;

        var matriculaResult = Matricula.Criar(
            id: 0,
            aluno: aluno,
            plano: plano,
            dataInicio: inicio,
            objetivo: "Luciano Coelho",
            restricoesMedicas: restricoes,
            laudoMedico: laudo,
            observacoesRestricoes: obsRestricao ?? DatabaseLabel
        );

        if (matriculaResult.IsFailure)
            throw new Exception($"Falha ao criar Matricula no Helper: {string.Join(", ", matriculaResult.Notifications.Select(n => n.Mensagem))}");

        return await _matriculaRepo.Adicionar(matriculaResult.Value!);
    }

    [Fact]
    public async Task Matricula_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var restricoesComb = MatriculaRestricoes.Diabetes | MatriculaRestricoes.PressaoAlta;
        var laudo = Arquivo.Criar(new byte[] { 100, 101, 102 }).Value;

        var inserida = await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Mensal, restricoes: restricoesComb, laudo: laudo);

        Assert.NotNull(inserida);
        Assert.True(inserida.Id > 0);
        Assert.Equal(aluno.Id, inserida.AlunoId);
        Assert.Equal(MatriculaPlano.Mensal, inserida.Plano);
        Assert.Equal(restricoesComb, inserida.RestricoesMedicas);
        Assert.True(inserida.RestricoesMedicas.HasFlag(MatriculaRestricoes.Diabetes));
        Assert.True(inserida.RestricoesMedicas.HasFlag(MatriculaRestricoes.PressaoAlta));

        var obtida = await _matriculaRepo.ObterPorId(inserida.Id);
        Assert.NotNull(obtida);
        Assert.Equal(inserida.Id, obtida.Id);
        Assert.Equal(aluno.Id, obtida.AlunoId);
        Assert.Equal(MatriculaPlano.Mensal, obtida.Plano);
        Assert.Equal(restricoesComb, obtida.RestricoesMedicas);
        Assert.NotNull(obtida.LaudoMedico);
        Assert.Equal(laudo!.Conteudo, obtida.LaudoMedico.Conteudo);
    }

    [Fact]
    public async Task Matricula_RestricoesMedicas_ComVariacoesMultiplaEscolha_PersisteEObtemCorretamente()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var laudo = Arquivo.Criar(new byte[] { 10, 20, 30, 40, 50 }).Value!;
        var restricoesMultiplas = MatriculaRestricoes.PressaoAlta | MatriculaRestricoes.Labirintite | MatriculaRestricoes.ProblemasRespiratorios | MatriculaRestricoes.RemedioContinuo;

        var matricula = await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Semestral, restricoes: restricoesMultiplas, laudo: laudo);
        var obtida = await _matriculaRepo.ObterPorId(matricula.Id);

        Assert.NotNull(obtida);
        Assert.Equal(restricoesMultiplas, obtida.RestricoesMedicas);
        Assert.True(obtida.RestricoesMedicas.HasFlag(MatriculaRestricoes.PressaoAlta));
        Assert.True(obtida.RestricoesMedicas.HasFlag(MatriculaRestricoes.Labirintite));
        Assert.True(obtida.RestricoesMedicas.HasFlag(MatriculaRestricoes.ProblemasRespiratorios));
        Assert.True(obtida.RestricoesMedicas.HasFlag(MatriculaRestricoes.RemedioContinuo));
        Assert.False(obtida.RestricoesMedicas.HasFlag(MatriculaRestricoes.Diabetes));
        Assert.False(obtida.RestricoesMedicas.HasFlag(MatriculaRestricoes.Alergias));
        Assert.NotNull(obtida.LaudoMedico);
        Assert.Equal(laudo.Conteudo, obtida.LaudoMedico.Conteudo);
    }

    [Fact]
    public async Task Matricula_ObterPorId_RetornaNuloQuandoInexistente()
    {
        Assert.Null(await _matriculaRepo.ObterPorId(999999));
    }

    [Fact]
    public async Task Matricula_ObterTodos_Sucesso()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        await CriarEInserirMatriculaAsync(aluno);
        var todas = await _matriculaRepo.ObterTodos();
        Assert.NotNull(todas);
        Assert.NotEmpty(todas);
    }

    [Fact]
    public async Task Matricula_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var matriculaInexistente = Matricula.Criar(
            id: 999999, aluno: aluno, plano: MatriculaPlano.Mensal, dataInicio: DateOnly.FromDateTime(DateTime.Today),
            objetivo: "Teste Inexistente", restricoesMedicas: MatriculaRestricoes.None, laudoMedico: null
        ).Value!;

        var ex = await Assert.ThrowsAsync<InfrastructureException>(() => _matriculaRepo.Atualizar(matriculaInexistente));
        Assert.Equal("REGISTRO_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Matricula_Atualizar_Sucesso()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var inserida = await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Mensal, restricoes: MatriculaRestricoes.Alergias);
        var novasRestricoes = MatriculaRestricoes.Alergias | MatriculaRestricoes.Diabetes | MatriculaRestricoes.Labirintite;
        var laudoAtualizado = Arquivo.Criar(new byte[] { 99, 88, 77 }).Value!;

        var matriculaAtualizada = Matricula.Criar(
            id: inserida.Id, aluno: aluno, plano: MatriculaPlano.Anual, dataInicio: inserida.DataInicio,
            objetivo: "SEU NOME", restricoesMedicas: novasRestricoes, laudoMedico: laudoAtualizado,
            observacoesRestricoes: DatabaseLabel
        ).Value!;

        var resultado = await _matriculaRepo.Atualizar(matriculaAtualizada);
        Assert.Equal(MatriculaPlano.Anual, resultado.Plano);
        Assert.Equal(novasRestricoes, resultado.RestricoesMedicas);

        var noBanco = await _matriculaRepo.ObterPorId(inserida.Id);
        Assert.NotNull(noBanco);
        Assert.Equal(MatriculaPlano.Anual, noBanco.Plano);
        Assert.Equal(novasRestricoes, noBanco.RestricoesMedicas);
    }

    [Fact]
    public async Task Matricula_Remover_Sucesso()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var inserida = await CriarEInserirMatriculaAsync(aluno);
        Assert.True(await _matriculaRepo.Remover(inserida.Id));
        Assert.Null(await _matriculaRepo.ObterPorId(inserida.Id));
    }

    [Fact]
    public async Task Matricula_Remover_RetornaFalseQuandoInexistente()
    {
        Assert.False(await _matriculaRepo.Remover(999999));
    }

    [Fact]
    public async Task Matricula_ObterPorAluno_FiltragemCorreta()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        await CriarEInserirMatriculaAsync(aluno);
        var matriculas = await _matriculaRepo.ObterPorAluno(aluno.Id);
        Assert.NotEmpty(matriculas);
        Assert.All(matriculas, m => Assert.Equal(aluno.Id, m.AlunoId));
    }

    [Fact]
    public async Task Matricula_ObterMatriculaAtivaPorAluno_E_PossuiMatriculaAtiva()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.False(await _matriculaRepo.PossuiMatriculaAtiva(aluno.Id));

        await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today));

        Assert.True(await _matriculaRepo.PossuiMatriculaAtiva(aluno.Id));
        var ativa = await _matriculaRepo.ObterMatriculaAtivaPorAluno(aluno.Id);
        Assert.NotNull(ativa);
        Assert.Equal(aluno.Id, ativa.AlunoId);
    }

    [Fact]
    public async Task Matricula_ObterAtivas_FiltragemCorreta()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Semestral, DateOnly.FromDateTime(DateTime.Today));

        var ativasGeral = await _matriculaRepo.ObterAtivas();
        Assert.NotEmpty(ativasGeral);

        var ativasPorAluno = await _matriculaRepo.ObterAtivas(aluno.Id);
        Assert.NotEmpty(ativasPorAluno);
        Assert.All(ativasPorAluno, m => Assert.Equal(aluno.Id, m.AlunoId));
    }

    [Fact]
    public async Task Matricula_ObterVencendoEmDias_RetornaMatriculasProximasDoVencimento()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var inicio = DateOnly.FromDateTime(DateTime.Today.AddDays(-25));
        await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Mensal, inicio);

        var vencendoEm30Dias = await _matriculaRepo.ObterVencendoEmDias(30);
        Assert.Contains(vencendoEm30Dias, m => m.AlunoId == aluno.Id);
    }

    [Fact]
    public async Task Matricula_ObterPorPlano_FiltragemCorreta()
    {
        var aluno = await AlunoInfrastructureTests.CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        await CriarEInserirMatriculaAsync(aluno, MatriculaPlano.Trimestral);

        var trimestrais = await _matriculaRepo.ObterPorPlano(MatriculaPlano.Trimestral);
        Assert.Contains(trimestrais, m => m.AlunoId == aluno.Id && m.Plano == MatriculaPlano.Trimestral);
    }
}