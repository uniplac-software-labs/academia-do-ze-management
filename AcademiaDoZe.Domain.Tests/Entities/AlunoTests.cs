// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class AlunoTests
{
    private readonly Logradouro _logradouroValido;
    private readonly Arquivo _fotoValida;

    public AlunoTests()
    {
        _logradouroValido = Logradouro.Criar(1, "88500000", "Rua Principal", "Centro", "Lages", "SC", "Brasil").Value!;
        _fotoValida = Arquivo.Criar([1, 2, 3]).Value!;
    }

    [Fact]
    public void Criar_AlunoValido_DeveRetornarSucesso()
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
        var result = Aluno.Criar(1, "João da Silva", "12345678901", dataNascimento, "49999999999", "joao@email.com", _logradouroValido, "100", "Apto 1", "Senha123", _fotoValida);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-11)]
    public void Criar_AlunoIdadeInvalida_DeveRetornarFalha(int anosSubtrair)
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(anosSubtrair));
        var result = Aluno.Criar(1, "Nome Teste", "12345678901", dataNascimento, "49999999999", "teste@email.com", _logradouroValido, "100", "", "Senha123", _fotoValida);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_AlunoNomeInvalido_DeveRetornarFalha(string nome)
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
        var result = Aluno.Criar(1, nome, "12345678901", dataNascimento, "49999999999", "teste@email.com", _logradouroValido, "100", "", "Senha123", _fotoValida);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("cpf_invalido")]
    public void Criar_AlunoCpfInvalido_DeveRetornarFalha(string cpf)
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
        var result = Aluno.Criar(1, "João Silva", cpf, dataNascimento, "49999999999", "teste@email.com", _logradouroValido, "100", "", "Senha123", _fotoValida);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("email_invalido")]
    [InlineData("email@")]
    public void Criar_AlunoEmailInvalido_DeveRetornarFalha(string email)
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
        var result = Aluno.Criar(1, "João Silva", "12345678901", dataNascimento, "49999999999", email, _logradouroValido, "100", "", "Senha123", _fotoValida);

        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("senhafraca")]
    public void Criar_AlunoSenhaInvalida_DeveRetornarFalha(string senha)
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20));
        var result = Aluno.Criar(1, "João Silva", "12345678901", dataNascimento, "49999999999", "joao@email.com", _logradouroValido, "100", "", senha, _fotoValida);

        Assert.True(result.IsFailure);
    }
}