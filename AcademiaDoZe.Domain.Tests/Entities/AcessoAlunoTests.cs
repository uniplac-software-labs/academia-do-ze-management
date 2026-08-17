// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoAlunoTests
{
    private readonly Aluno _alunoValido;

    public AcessoAlunoTests()
    {
        var logradouro = Logradouro.Criar(1, "88500000", "Rua", "Bairro", "Lages", "SC", "Brasil").Value!;
        var foto = Arquivo.Criar([1]).Value!;
        _alunoValido = Aluno.Criar(1, "Aluno", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-20)), "49999999999", "aluno@email.com", logradouro, "1", "", "Senha123", foto).Value!;
    }

    [Theory]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    [InlineData(12)]
    [InlineData(13)]
    [InlineData(14)]
    [InlineData(15)]
    [InlineData(16)]
    [InlineData(17)]
    [InlineData(18)]
    [InlineData(19)]
    [InlineData(20)]
    [InlineData(21)]
    [InlineData(22)]
    public void Criar_AcessoHorariosPermitidos_DeveRetornarSucesso(int hora)
    {
        var dataHora = new DateTime(2026, 8, 17, hora, 0, 0);
        var result = AcessoAluno.Criar(1, _alunoValido, dataHora);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(23)]
    public void Criar_AcessoHorariosProibidos_DeveRetornarFalha(int hora)
    {
        var dataHora = new DateTime(2026, 8, 17, hora, 0, 0);
        var result = AcessoAluno.Criar(1, _alunoValido, dataHora);
        Assert.True(result.IsFailure);
    }
}