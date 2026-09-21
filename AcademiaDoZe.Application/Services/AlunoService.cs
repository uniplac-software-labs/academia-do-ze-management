using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Application.Security;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class AlunoService : IAlunoService
{
    private readonly Func<IAlunoRepository> _repoFactory;
    private readonly Func<ILogradouroRepository>? _logradouroRepoFactory;

    public AlunoService(
        Func<IAlunoRepository> repoFactory,
        Func<ILogradouroRepository>? logradouroRepoFactory = null)
    {
        _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        _logradouroRepoFactory = logradouroRepoFactory;
    }

    public async Task<bool> CpfJaExisteAsync(string cpf, int? id = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;
        var result = Cpf.Criar(cpf);
        if (result.IsFailure) return false;
        return await _repoFactory().CpfJaExiste(result.Value!, id, cancellationToken);
    }

    public async Task<bool> EmailJaExisteAsync(string email, int? id = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        var result = Email.Criar(email);
        if (result.IsFailure) return false;
        return await _repoFactory().EmailJaExiste(result.Value!, id, cancellationToken);
    }

    public async Task<AlunoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var aluno = await _repoFactory().ObterPorId(id, cancellationToken);
        if (aluno == null) return null;

        if (_logradouroRepoFactory != null)
        {
            var logradouro = await _logradouroRepoFactory().ObterPorId(aluno.Endereco.LogradouroId, cancellationToken);
            return aluno.ToDto(logradouro);
        }

        return aluno.ToDto();
    }

    public async Task<IEnumerable<AlunoDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var alunos = (await _repoFactory().ObterTodos(cancellationToken)).ToList();
        if (alunos.Count == 0) return [];

        if (_logradouroRepoFactory == null)
            return [.. alunos.Select(a => a.ToDto())];

        var logs = new Dictionary<int, AcademiaDoZe.Domain.Entities.Logradouro>();

        foreach (var id in alunos.Select(a => a.Endereco.LogradouroId).Distinct())
        {
            var log = await _logradouroRepoFactory().ObterPorId(id, cancellationToken);
            if (log != null) logs[id] = log;
        }

        return [.. alunos.Select(a => a.ToDto(logs.GetValueOrDefault(a.Endereco.LogradouroId)))];
    }

    public async Task<AlunoDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF não pode ser vazio.", nameof(cpf));

        var result = Cpf.Criar(cpf);
        if (result.IsFailure)
            throw new ArgumentException(
                $"CPF inválido: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}",
                nameof(cpf));

        var aluno = await _repoFactory().ObterPorCpf(result.Value!, cancellationToken);
        return aluno?.ToDto();
    }

    public async Task<AlunoDto?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email não pode ser vazio.", nameof(email));

        var result = Email.Criar(email);
        if (result.IsFailure)
            throw new ArgumentException(
                $"Email inválido: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}",
                nameof(email));

        var aluno = await _repoFactory().ObterPorEmail(result.Value!, cancellationToken);
        return aluno?.ToDto();
    }

    public async Task<IEnumerable<AlunoDto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome não pode ser vazio.", nameof(nome));

        var alunos = await _repoFactory().ObterPorNome(nome.Trim(), cancellationToken);
        return [.. alunos.Select(a => a.ToDto())];
    }

    public async Task<bool> TrocarSenhaAsync(int id, string novaSenha, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(novaSenha))
            throw new ArgumentException("Nova senha não pode ser vazia.", nameof(novaSenha));

        var validacao = Senha.Criar(novaSenha);
        if (validacao.IsFailure)
            throw new ArgumentException(
                $"Nova senha inválida: {string.Join(", ", validacao.Notifications.Select(n => n.Mensagem))}",
                nameof(novaSenha));

        var senhaHash = Senha.Criar(PasswordHasher.Hash(novaSenha));
        if (senhaHash.IsFailure)
            throw new InvalidOperationException("Falha ao gerar hash da nova senha.");

        return await _repoFactory().TrocarSenha(id, senhaHash.Value!, cancellationToken);
    }

    public async Task<AlunoDto> AdicionarAsync(AlunoDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cpfResult = Cpf.Criar(dto.Cpf);
        if (cpfResult.IsFailure)
            throw new ArgumentException(
                $"CPF inválido: {string.Join(", ", cpfResult.Notifications.Select(n => n.Mensagem))}",
                nameof(dto));

        if (await _repoFactory().CpfJaExiste(cpfResult.Value!, null, cancellationToken))
            throw new InvalidOperationException($"Já existe um aluno cadastrado com o CPF {dto.Cpf}.");

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var emailResult = Email.Criar(dto.Email);
            if (emailResult.IsFailure)
                throw new ArgumentException(
                    $"Email inválido: {string.Join(", ", emailResult.Notifications.Select(n => n.Mensagem))}",
                    nameof(dto));

            if (await _repoFactory().EmailJaExiste(emailResult.Value!, null, cancellationToken))
                throw new InvalidOperationException($"Já existe um aluno cadastrado com o Email {dto.Email}.");
        }

        if (string.IsNullOrWhiteSpace(dto.Senha))
            throw new ArgumentException("Senha é obrigatória para cadastrar o aluno.", nameof(dto));

        var senhaResult = Senha.Criar(dto.Senha);
        if (senhaResult.IsFailure)
            throw new ArgumentException(
                $"Senha não atende aos requisitos mínimos: {string.Join(", ", senhaResult.Notifications.Select(n => n.Mensagem))}",
                nameof(dto));

        dto.Senha = PasswordHasher.Hash(dto.Senha);

        AcademiaDoZe.Domain.Entities.Logradouro? logradouro = null;

        if (_logradouroRepoFactory != null && dto.Endereco != null && dto.Endereco.Id > 0)
        {
            logradouro = await _logradouroRepoFactory().ObterPorId(dto.Endereco.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Logradouro com ID {dto.Endereco.Id} não encontrado.");
        }

        var adicionado = await _repoFactory().Adicionar(dto.ToEntity(logradouro), cancellationToken);
        return adicionado.ToDto(logradouro);
    }

    public async Task<AlunoDto> AtualizarAsync(AlunoDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existente = await _repoFactory().ObterPorId(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Aluno com ID {dto.Id} não encontrado.");

        var cpfResult = Cpf.Criar(dto.Cpf);
        if (cpfResult.IsFailure)
            throw new ArgumentException(
                $"CPF inválido: {string.Join(", ", cpfResult.Notifications.Select(n => n.Mensagem))}",
                nameof(dto));

        if (await _repoFactory().CpfJaExiste(cpfResult.Value!, dto.Id, cancellationToken))
            throw new InvalidOperationException($"Já existe outro aluno cadastrado com o CPF {dto.Cpf}.");

        if (!string.IsNullOrWhiteSpace(dto.Email) &&
            !string.Equals(dto.Email, existente.Email.Valor, StringComparison.OrdinalIgnoreCase))
        {
            var emailResult = Email.Criar(dto.Email);
            if (emailResult.IsFailure)
                throw new ArgumentException(
                    $"Email inválido: {string.Join(", ", emailResult.Notifications.Select(n => n.Mensagem))}",
                    nameof(dto));

            if (await _repoFactory().EmailJaExiste(emailResult.Value!, dto.Id, cancellationToken))
                throw new InvalidOperationException($"Já existe outro aluno cadastrado com o Email {dto.Email}.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Senha))
        {
            var senhaResult = Senha.Criar(dto.Senha);
            if (senhaResult.IsFailure)
                throw new ArgumentException(
                    $"Senha não atende aos requisitos mínimos: {string.Join(", ", senhaResult.Notifications.Select(n => n.Mensagem))}",
                    nameof(dto));

            dto.Senha = PasswordHasher.Hash(dto.Senha);
        }

        AcademiaDoZe.Domain.Entities.Logradouro? logradouro = null;
        int logradouroId = dto.Endereco?.Id > 0 ? dto.Endereco.Id : existente.Endereco.LogradouroId;

        if (_logradouroRepoFactory != null && logradouroId > 0)
        {
            logradouro = await _logradouroRepoFactory().ObterPorId(logradouroId, cancellationToken)
                ?? throw new KeyNotFoundException($"Logradouro com ID {logradouroId} não encontrado.");
        }

        var atualizado = await _repoFactory().Atualizar(existente.UpdateFromDto(dto, logradouro), cancellationToken);
        return atualizado.ToDto(logradouro);
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _repoFactory().ObterPorId(id, cancellationToken);
        if (existente == null) return false;
        return await _repoFactory().Remover(id, cancellationToken);
    }
}
