// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Services;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Services;

public class NormalizacaoServiceTests
{
    [Theory]
    [InlineData("  teste   com   espacos  ", "teste com espacos")]
    [InlineData("semEspacos", "semEspacos")]
    public void LimparEspacos_DeveRemoverEspacosDuplicados(string input, string esperado)
    {
        var resultado = NormalizacaoService.LimparEspacos(input);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("123.456.789-00", "12345678900")]
    [InlineData("abc 123 def 456", "123456")]
    public void LimparEDigitos_DeveRetornarApenasNumeros(string input, string esperado)
    {
        var resultado = NormalizacaoService.LimparEDigitos(input);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("sc", "SC")]
    [InlineData("  sp  ", "SP")]
    public void ParaMaiusculo_DeveConverterParaCaixaAlta(string input, string esperado)
    {
        var resultado = NormalizacaoService.ParaMaiusculo(NormalizacaoService.LimparTodosEspacos(input));
        Assert.Equal(esperado, resultado);
    }
}