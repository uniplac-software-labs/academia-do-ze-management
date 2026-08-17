// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class LogradouroTests
{
    [Fact]
    public void Criar_LogradouroValido_DeveRetornarSucesso()
    {
        var result = Logradouro.Criar(1, "88500000", "Rua das Flores", "Centro", "Lages", "SC", "Brasil");
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Theory]
    [InlineData("", "Rua", "Bairro", "Cidade", "SC", "Brasil")]
    [InlineData("88500000", "", "Bairro", "Cidade", "SC", "Brasil")]
    [InlineData("88500000", "Rua", "", "Cidade", "SC", "Brasil")]
    [InlineData("88500000", "Rua", "Bairro", "", "SC", "Brasil")]
    [InlineData("88500000", "Rua", "Bairro", "Cidade", "", "Brasil")]
    [InlineData("88500000", "Rua", "Bairro", "Cidade", "SC", "")]
    public void Criar_LogradouroInvalido_DeveRetornarFalha(string cep, string nome, string bairro, string cidade, string estado, string pais)
    {
        var result = Logradouro.Criar(1, cep, nome, bairro, cidade, estado, pais);
        Assert.True(result.IsFailure);
    }
}