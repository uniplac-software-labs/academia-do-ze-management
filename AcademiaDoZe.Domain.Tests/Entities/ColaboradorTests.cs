// Pedro Henrique dos Santos
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using Xunit;

namespace AcademiaDoZe.Domain.Tests.Entities;

public class ColaboradorTests
{
    private readonly Logradouro _logradouroValido;
    private readonly Arquivo _fotoValida;

    public ColaboradorTests()
    {
        _logradouroValido = Logradouro.Criar(1, "88500000", "Rua Principal", "Centro", "Lages", "SC", "Brasil").Value!;
        _fotoValida = Arquivo.Criar([1, 2, 3]).Value!;
    }

    [Fact]
    public void Criar_ColaboradorValido_DeveRetornarSucesso()
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));
        var dataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));

        var result = Colaborador.Criar(1, "Carlos Admin", "12345678901", dataNascimento, "49999999999", "carlos@email.com", _logradouroValido, "10", "", "Senha123", _fotoValida, dataAdmissao, ColaboradorTipo.Administrador, ColaboradorVinculo.CLT);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public void Criar_AdministradorNaoCLT_DeveRetornarFalha()
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-30));
        var dataAdmissao = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));

        var result = Colaborador.Criar(1, "Carlos Admin", "12345678901", dataNascimento, "49999999999", "carlos@email.com", _logradouroValido, "10", "", "Senha123", _fotoValida, dataAdmissao, ColaboradorTipo.Administrador, ColaboradorVinculo.Estagio);

        Assert.True(result.IsFailure);
    }
}