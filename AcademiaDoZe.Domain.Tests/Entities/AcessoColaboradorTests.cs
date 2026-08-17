// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AcessoColaboradorTests
{
    private readonly Colaborador _colaboradorValido;

    public AcessoColaboradorTests()
    {
        var logradouro = Logradouro.Criar(1, "88500000", "Rua", "Bairro", "Lages", "SC", "Brasil").Value!;
        var foto = Arquivo.Criar([1]).Value!;
        _colaboradorValido = Colaborador.Criar(1, "Colaborador", "12345678901", DateOnly.FromDateTime(DateTime.Today.AddYears(-25)), "49999999999", "colab@email.com", logradouro, "1", "", "Senha123", foto, DateOnly.FromDateTime(DateTime.Today.AddMonths(-2)), ColaboradorTipo.Atendente, ColaboradorVinculo.CLT).Value!;
    }

    [Fact]
    public void Criar_AcessoNoHorarioPermitido_DeveRetornarSucesso()
    {
        var dataHora = new DateTime(2026, 8, 17, 8, 0, 0);
        var result = AcessoColaborador.Criar(1, _colaboradorValido, dataHora);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(4, 0, 0)]
    [InlineData(23, 0, 0)]
    public void Criar_AcessoForaDoHorarioPermitido_DeveRetornarFalha(int hora, int minuto, int segundo)
    {
        var dataHora = new DateTime(2026, 8, 17, hora, minuto, segundo);
        var result = AcessoColaborador.Criar(1, _colaboradorValido, dataHora);

        Assert.True(result.IsFailure);
    }
}