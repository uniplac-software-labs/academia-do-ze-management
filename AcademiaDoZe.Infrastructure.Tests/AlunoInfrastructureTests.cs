using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Exceptions;
using AcademiaDoZe.Infrastructure.Repositories;
using Xunit;

namespace AcademiaDoZe.Infrastructure.Tests;

public class AlunoInfrastructureTests : TestBase
{
    private readonly LogradouroRepository _logradouroRepo;
    private readonly AlunoRepository _alunoRepo;

    public AlunoInfrastructureTests()
    {
        _logradouroRepo = new LogradouroRepository(ConnectionString, DatabaseType);
        _alunoRepo = new AlunoRepository(ConnectionString, DatabaseType);
    }

    // Pedro Henrique dos Santos
    internal static async Task<Aluno> CriarEInserirAlunoAsync(AlunoRepository alunoRepo, LogradouroRepository logradouroRepo)
    {
        var logradouro = await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(logradouroRepo);
        var foto = Arquivo.Criar(new byte[] { 1, 2, 3, 4 }).Value!;
        var alunoResult = Aluno.Criar(
            id: 0,
            nome: "Luciano",
            cpf: GerarCpf(),
            dataNascimento: new DateOnly(2000, 3, 10),
            telefone: GerarTelefone(),
            email: GerarEmail(),
            endereco: logradouro,
            numero: "100",
            complemento: "Coelho",
            senha: "SenhaValida123" + DatabaseLabel,
            foto: foto
        );

        if (alunoResult.IsFailure)
            throw new Exception($"Falha ao criar Aluno: {string.Join(", ", alunoResult.Notifications.Select(n => n.Mensagem))}");

        return await alunoRepo.Adicionar(alunoResult.Value!);
    }

    [Fact]
    public async Task Aluno_Adicionar_E_ObterPorId_Sucesso()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.NotNull(aluno);
        Assert.True(aluno.Id > 0);

        var obtido = await _alunoRepo.ObterPorId(aluno.Id);
        Assert.NotNull(obtido);
        Assert.Equal(aluno.Id, obtido.Id);
        Assert.Equal(aluno.Cpf.Valor, obtido.Cpf.Valor);
        Assert.Equal(aluno.Nome, obtido.Nome);
        Assert.Equal(aluno.Email.Valor, obtido.Email.Valor);
        Assert.NotNull(obtido.Endereco);
        Assert.Equal(aluno.Endereco.LogradouroId, obtido.Endereco.LogradouroId);
    }

    [Fact]
    public async Task Aluno_ObterPorId_RetornaNuloQuandoInexistente()
    {
        var obtido = await _alunoRepo.ObterPorId(999999);
        Assert.Null(obtido);
    }

    [Fact]
    public async Task Aluno_ObterTodos_Sucesso()
    {
        await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var todos = await _alunoRepo.ObterTodos();
        Assert.NotNull(todos);
        Assert.NotEmpty(todos);
    }

    [Fact]
    public async Task Aluno_Atualizar_Sucesso()
    {
        var logradouro = await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(_logradouroRepo);
        var foto = Arquivo.Criar(new byte[] { 1, 2, 3 }).Value!;
        var aluno = await _alunoRepo.Adicionar(Aluno.Criar(
            0, "Aluno Original", GerarCpf(), new DateOnly(2000, 3, 10),
            GerarTelefone(), GerarEmail(), logradouro, "100", "Ap 1", "Senha123", foto).Value!);

        var novoNome = "Aluno Editado " + Guid.NewGuid().ToString("N")[..5];
        var alunoAtualizado = Aluno.Criar(
            id: aluno.Id, nome: novoNome, cpf: aluno.Cpf.Valor, dataNascimento: aluno.DataNascimento,
            telefone: aluno.Telefone.Valor, email: aluno.Email.Valor, endereco: logradouro,
            numero: "150", complemento: "Ap 2", senha: aluno.Senha.Valor, foto: aluno.Foto
        ).Value!;

        var resultado = await _alunoRepo.Atualizar(alunoAtualizado);
        Assert.Equal(novoNome, resultado.Nome);

        var noBanco = await _alunoRepo.ObterPorId(aluno.Id);
        Assert.NotNull(noBanco);
        Assert.Equal(novoNome, noBanco.Nome);
    }

    [Fact]
    public async Task Aluno_Atualizar_LancaExcecaoQuandoInexistente()
    {
        var logradouro = await LogradouroInfrastructureTests.CriarEInserirLogradouroAsync(_logradouroRepo);
        var foto = Arquivo.Criar(new byte[] { 1, 2 }).Value!;
        var alunoInexistente = Aluno.Criar(
            id: 999999, nome: "Inexistente", cpf: GerarCpf(), dataNascimento: new DateOnly(2000, 1, 1),
            telefone: GerarTelefone(), email: GerarEmail(), endereco: logradouro, numero: "1", complemento: "",
            senha: "SenhaValida123", foto: foto
        ).Value!;

        var ex = await Assert.ThrowsAsync<InfrastructureException>(() => _alunoRepo.Atualizar(alunoInexistente));
        Assert.Equal("REGISTRO_NAO_ENCONTRADO", ex.ErrorCode);
    }

    [Fact]
    public async Task Aluno_Remover_Sucesso()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.True(await _alunoRepo.Remover(aluno.Id));
        Assert.Null(await _alunoRepo.ObterPorId(aluno.Id));
    }

    [Fact]
    public async Task Aluno_Remover_RetornaFalseQuandoInexistente()
    {
        Assert.False(await _alunoRepo.Remover(999999));
    }

    [Fact]
    public async Task Aluno_ObterPorCpf_SucessoENulo()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var obtido = await _alunoRepo.ObterPorCpf(aluno.Cpf);
        Assert.NotNull(obtido);
        Assert.Equal(aluno.Id, obtido.Id);

        var cpfInexistente = Cpf.Criar(GerarCpf()).Value!;
        Assert.Null(await _alunoRepo.ObterPorCpf(cpfInexistente));
    }

    [Fact]
    public async Task Aluno_ObterPorEmail_SucessoENulo()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var obtido = await _alunoRepo.ObterPorEmail(aluno.Email);
        Assert.NotNull(obtido);

        var emailInexistente = Email.Criar(GerarEmail()).Value!;
        Assert.Null(await _alunoRepo.ObterPorEmail(emailInexistente));
    }

    [Fact]
    public async Task Aluno_CpfJaExiste_ValidacaoCorreta()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.True(await _alunoRepo.CpfJaExiste(aluno.Cpf));
        Assert.False(await _alunoRepo.CpfJaExiste(aluno.Cpf, aluno.Id));
        Assert.False(await _alunoRepo.CpfJaExiste(Cpf.Criar(GerarCpf()).Value!));
    }

    [Fact]
    public async Task Aluno_EmailJaExiste_ValidacaoCorreta()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        Assert.True(await _alunoRepo.EmailJaExiste(aluno.Email));
        Assert.False(await _alunoRepo.EmailJaExiste(aluno.Email, aluno.Id));
        Assert.False(await _alunoRepo.EmailJaExiste(Email.Criar(GerarEmail()).Value!));
    }

    [Fact]
    public async Task Aluno_ObterPorNome_FiltragemCorreta()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var resultados = await _alunoRepo.ObterPorNome(aluno.Nome);
        Assert.Contains(resultados, a => a.Id == aluno.Id);
    }

    [Fact]
    public async Task Aluno_TrocarSenha_SucessoEFalha()
    {
        var aluno = await CriarEInserirAlunoAsync(_alunoRepo, _logradouroRepo);
        var novaSenha = Senha.Criar("NovaSenhaAluno123").Value!;
        Assert.True(await _alunoRepo.TrocarSenha(aluno.Id, novaSenha));
        var atualizado = await _alunoRepo.ObterPorId(aluno.Id);
        Assert.Equal("NovaSenhaAluno123", atualizado!.Senha.Valor);
        Assert.False(await _alunoRepo.TrocarSenha(999999, novaSenha));
    }
}