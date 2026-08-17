// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class MatriculaTests
{
    private readonly Aluno _alunoMaior;
    private readonly Aluno _alunoMenor16;
    private readonly Arquivo _laudo;

    public MatriculaTests()
    {
        var logradouro = Logradouro.Criar(1, "88500000", "Rua", "Bairro", "Lages", "SC", "Brasil").Value!;
        var foto = Arquivo.Criar([1]).Value!;
        _laudo = Arquivo.Criar([2]).Value!;

        _alunoMaior = Aluno.Criar(1, "Aluno Maior", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)), "49999999999", "maior@email.com", logradouro, "1", "", "Senha123", foto).Value!;
        _alunoMenor16 = Aluno.Criar(2, "Aluno Menor", "12345678902", DateOnly.FromDateTime(DateTime.Today.AddYears(-14)), "49999999999", "menor@email.com", logradouro, "1", "", "Senha123", foto).Value!;
    }

    [Theory]
    [InlineData(MatriculaPlano.Mensal)]
    [InlineData(MatriculaPlano.Trimestral)]
    [InlineData(MatriculaPlano.Semestral)]
    [InlineData(MatriculaPlano.Anual)]
    public void Criar_MatriculaTodosPlanos_DeveRetornarSucesso(MatriculaPlano plano)
    {
        var result = Matricula.Criar(1, _alunoMaior, plano, DateOnly.FromDateTime(DateTime.Today), "Condicionamento Físico", MatriculaRestricoes.None, null);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Criar_MenorDe16SemLaudo_DeveRetornarFalha()
    {
        var result = Matricula.Criar(1, _alunoMenor16, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), "Hipertrofia", MatriculaRestricoes.None, null);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Criar_MenorDe16ComLaudo_DeveRetornarSucesso()
    {
        var result = Matricula.Criar(1, _alunoMenor16, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), "Hipertrofia", MatriculaRestricoes.None, _laudo);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(MatriculaRestricoes.Diabetes)]
    [InlineData(MatriculaRestricoes.PressaoAlta)]
    [InlineData(MatriculaRestricoes.Labirintite)]
    [InlineData(MatriculaRestricoes.Alergias)]
    [InlineData(MatriculaRestricoes.ProblemasRespiratorios)]
    [InlineData(MatriculaRestricoes.RemedioContinuo)]
    public void Criar_ComRestricaoSemLaudo_DeveRetornarFalha(MatriculaRestricoes restricao)
    {
        var result = Matricula.Criar(1, _alunoMaior, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), "Saúde", restricao, null);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(MatriculaRestricoes.Diabetes)]
    [InlineData(MatriculaRestricoes.PressaoAlta)]
    [InlineData(MatriculaRestricoes.Labirintite)]
    [InlineData(MatriculaRestricoes.Alergias)]
    [InlineData(MatriculaRestricoes.ProblemasRespiratorios)]
    [InlineData(MatriculaRestricoes.RemedioContinuo)]
    public void Criar_ComRestricaoEComLaudo_DeveRetornarSucesso(MatriculaRestricoes restricao)
    {
        var result = Matricula.Criar(1, _alunoMaior, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), "Saúde", restricao, _laudo);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ObjetivoVazio_DeveRetornarFalha(string objetivo)
    {
        var result = Matricula.Criar(1, _alunoMaior, MatriculaPlano.Mensal, DateOnly.FromDateTime(DateTime.Today), objetivo, MatriculaRestricoes.None, null);
        Assert.True(result.IsFailure);
    }
}