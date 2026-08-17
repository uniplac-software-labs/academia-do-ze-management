// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.ValueObjects;

public class ValueObjectsTests
{
    [Theory]
    [InlineData("88500000")]
    [InlineData("88.500-000")]
    [InlineData("01001-000")]
    [InlineData("70000000")]
    [InlineData("88000-000")]
    public void Cep_Valido_DeveRetornarSucesso(string valor)
    {
        var result = Cep.Criar(valor);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("ABCDEFGH")]
    [InlineData("88500-00A")]
    public void Cep_Invalido_DeveRetornarFalha(string valor)
    {
        var result = Cep.Criar(valor);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("12345678901")]
    [InlineData("123.456.789-01")]
    [InlineData("98765432100")]
    [InlineData("00000000000")]
    [InlineData("111.222.333-44")]
    public void Cpf_Valido_DeveRetornarSucesso(string valor)
    {
        var result = Cpf.Criar(valor);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("ABCDEFGHIJK")]
    [InlineData("123.456.789-0A")]
    public void Cpf_Invalido_DeveRetornarFalha(string valor)
    {
        var result = Cpf.Criar(valor);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("teste@email.com")]
    [InlineData("usuario.dev@dominio.com.br")]
    [InlineData("aluno123@academia.org")]
    [InlineData("admin@empresa.net")]
    [InlineData("contato@sub.dominio.com")]
    public void Email_Valido_DeveRetornarSucesso(string valor)
    {
        var result = Email.Criar(valor);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalido")]
    [InlineData("teste@")]
    [InlineData("@dominio.com")]
    [InlineData("teste@dominio")]
    [InlineData("teste@.com")]
    [InlineData("teste@com.")]
    public void Email_Invalido_DeveRetornarFalha(string valor)
    {
        var result = Email.Criar(valor);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("Senha123")]
    [InlineData("Abcdef1")]
    [InlineData("MINHASENHA1")]
    [InlineData("Forte@2026")]
    [InlineData("Zezinho1")]
    public void Senha_Valida_DeveRetornarSucesso(string valor)
    {
        var result = Senha.Criar(valor);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("12345")]
    [InlineData("Abc12")]
    [InlineData("senhafraca123")]
    [InlineData("12345678")]
    public void Senha_Invalida_DeveRetornarFalha(string valor)
    {
        var result = Senha.Criar(valor);
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("49999999999")]
    [InlineData("(49) 99999-9999")]
    [InlineData("11988887777")]
    [InlineData("(11) 98888-7777")]
    [InlineData("47911112222")]
    public void Telefone_Valido_DeveRetornarSucesso(string valor)
    {
        var result = Telefone.Criar(valor);
        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("ABCDEFGHIJK")]
    public void Telefone_Invalido_DeveRetornarFalha(string valor)
    {
        var result = Telefone.Criar(valor);
        Assert.True(result.IsFailure);
    }
}