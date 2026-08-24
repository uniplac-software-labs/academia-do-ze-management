//Pedro Henrique dos Santos

using System.Linq;
using System.Threading.Tasks;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class LogradouroInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _repository;

    public LogradouroInfrastructureTests()
    {
        _repository = new LogradouroRepository(ConnectionString, DatabaseType);
    }

    [Fact]
    public async Task Adicionar_E_ObterPorId_DeveRetornarLogradouroComSucesso()
    {
        // Arrange
        string cepGerado = GerarCep();
        var logradouroDomain = Logradouro.Criar(0, cepGerado, "Rua das Flores", "Centro", "Campinas", "SP", "Brasil").Value!;

        // Act
        var inserido = await _repository.Adicionar(logradouroDomain);
        var consultado = await _repository.ObterPorId(inserido.Id);

        // Assert
        Assert.NotNull(consultado);
        Assert.True(consultado.Id > 0);
        Assert.Equal(cepGerado, consultado.Cep.Valor);
        Assert.Equal("Rua das Flores", consultado.Nome);
        Assert.Equal("Centro", consultado.Bairro);
        Assert.Equal("Campinas", consultado.Cidade);
        Assert.Equal("SP", consultado.Estado);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarListaDeLogradouros()
    {
        // Arrange
        var log1 = Logradouro.Criar(0, GerarCep(), "Rua Um", "Centro", "Campinas", "SP", "Brasil").Value!;
        var log2 = Logradouro.Criar(0, GerarCep(), "Rua Dois", "Taquaral", "Campinas", "SP", "Brasil").Value!;

        await _repository.Adicionar(log1);
        await _repository.Adicionar(log2);

        // Act
        var todos = await _repository.ObterTodos();

        // Assert
        Assert.NotNull(todos);
        Assert.True(todos.Count() >= 2);
    }

    [Fact]
    public async Task Atualizar_DeveModificarRegistroExistente()
    {
        // Arrange
        string cepGerado = GerarCep();
        var inserido = await _repository.Adicionar(Logradouro.Criar(0, cepGerado, "Nome Antigo", "Bairro", "Cidade", "SP", "Brasil").Value!);
        var logradouroParaAtualizar = Logradouro.Criar(inserido.Id, cepGerado, "Nome Novo", "Bairro", "Cidade", "SP", "Brasil").Value!;

        // Act
        var atualizado = await _repository.Atualizar(logradouroParaAtualizar);
        var consultado = await _repository.ObterPorId(inserido.Id);

        // Assert
        Assert.NotNull(atualizado);
        Assert.NotNull(consultado);
        Assert.Equal("Nome Novo", consultado.Nome);
    }

    [Fact]
    public async Task Remover_DeveExcluirRegistro()
    {
        // Arrange
        var inserido = await _repository.Adicionar(Logradouro.Criar(0, GerarCep(), "Rua Para Deletar", "Bairro", "Cidade", "SP", "Brasil").Value!);

        // Act
        bool removido = await _repository.Remover(inserido.Id);
        var consultado = await _repository.ObterPorId(inserido.Id);

        // Assert
        Assert.True(removido);
        Assert.Null(consultado);
    }

    [Fact]
    public async Task ObterPorCep_DeveRetornarRegistroCorreto()
    {
        // Arrange
        string cepStr = GerarCep();
        await _repository.Adicionar(Logradouro.Criar(0, cepStr, "Rua CEP Teste", "Bairro", "Cidade", "SP", "Brasil").Value!);
        var cep = Cep.Criar(cepStr).Value!;

        // Act
        var consultado = await _repository.ObterPorCep(cep);

        // Assert
        Assert.NotNull(consultado);
        Assert.Equal(cepStr, consultado.Cep.Valor);
    }

    [Fact]
    public async Task CepJaExiste_DeveValidarExistencia()
    {
        // Arrange
        string cepStr = GerarCep();
        var inserido = await _repository.Adicionar(Logradouro.Criar(0, cepStr, "Rua Unica", "Bairro", "Cidade", "SP", "Brasil").Value!);
        var cep = Cep.Criar(cepStr).Value!;

        // Act & Assert
        Assert.True(await _repository.CepJaExiste(cep));
        Assert.False(await _repository.CepJaExiste(cep, inserido.Id));
    }

    [Fact]
    public async Task ObterPorCidadeEBairro_DevemFiltrarCorretamente()
    {
        // Arrange
        string cidadeUnica = $"Cidade_{GerarCep()}";
        string bairroUnico = $"Bairro_{GerarCep()}";

        await _repository.Adicionar(Logradouro.Criar(0, GerarCep(), "Rua A", bairroUnico, cidadeUnica, "SP", "Brasil").Value!);
        await _repository.Adicionar(Logradouro.Criar(0, GerarCep(), "Rua B", bairroUnico, cidadeUnica, "SP", "Brasil").Value!);

        // Act
        var porCidade = await _repository.ObterPorCidade(cidadeUnica);
        var porBairro = await _repository.ObterPorBairro(cidadeUnica, bairroUnico);

        // Assert
        Assert.Equal(2, porCidade.Count());
        Assert.Equal(2, porBairro.Count());
    }
}