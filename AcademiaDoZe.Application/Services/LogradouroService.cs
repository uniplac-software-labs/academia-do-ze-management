using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Services;

public class LogradouroService : ILogradouroService
{
    private readonly Func<ILogradouroRepository> _repoFactory;

    public LogradouroService(Func<ILogradouroRepository> repoFactory)
    {
        _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
    }

    public async Task<bool> CepJaExisteAsync(string cep, int? id = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cep)) return false;

        var cepResult = Cep.Criar(cep);
        if (cepResult.IsFailure) return false;

        return await _repoFactory().CepJaExiste(cepResult.Value!, id, cancellationToken);
    }

    public async Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var logradouro = await _repoFactory().ObterPorId(id, cancellationToken);
        return logradouro?.ToDto();
    }

    public async Task<IEnumerable<LogradouroDto>> ObterTodosAsync(CancellationToken cancellationToken = default)
    {
        var logradouros = await _repoFactory().ObterTodos(cancellationToken);
        return [.. logradouros.Select(l => l.ToDto())];
    }

    public async Task<LogradouroDto?> ObterPorCepAsync(string cep, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cep))
            throw new ArgumentException("CEP não pode ser vazio.", nameof(cep));

        var cepResult = Cep.Criar(cep);

        if (cepResult.IsFailure)
            throw new ArgumentException(
                $"CEP inválido: {string.Join(", ", cepResult.Notifications.Select(n => n.Mensagem))}",
                nameof(cep));

        var logradouro = await _repoFactory().ObterPorCep(cepResult.Value!, cancellationToken);
        return logradouro?.ToDto();
    }

    public async Task<IEnumerable<LogradouroDto>> ObterPorCidadeAsync(string cidade, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade não pode ser vazia.", nameof(cidade));

        var logradouros = await _repoFactory().ObterPorCidade(cidade.Trim(), cancellationToken);
        return [.. logradouros.Select(l => l.ToDto())];
    }

    public async Task<IEnumerable<LogradouroDto>> ObterPorBairroAsync(
        string cidade,
        string bairro,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade não pode ser vazia.", nameof(cidade));

        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro não pode ser vazio.", nameof(bairro));

        var logradouros = await _repoFactory().ObterPorBairro(cidade.Trim(), bairro.Trim(), cancellationToken);
        return [.. logradouros.Select(l => l.ToDto())];
    }

    public async Task<LogradouroDto> AdicionarAsync(LogradouroDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var cepResult = Cep.Criar(dto.Cep);

        if (cepResult.IsFailure)
            throw new ArgumentException(
                $"CEP inválido: {string.Join(", ", cepResult.Notifications.Select(n => n.Mensagem))}",
                nameof(dto));

        if (await _repoFactory().CepJaExiste(cepResult.Value!, null, cancellationToken))
            throw new InvalidOperationException($"Já existe um logradouro cadastrado com o CEP {dto.Cep}.");

        var adicionado = await _repoFactory().Adicionar(dto.ToEntity(), cancellationToken);
        return adicionado.ToDto();
    }

    public async Task<LogradouroDto> AtualizarAsync(LogradouroDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existente = await _repoFactory().ObterPorId(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Logradouro com ID {dto.Id} não encontrado.");

        var cepResult = Cep.Criar(dto.Cep);

        if (cepResult.IsFailure)
            throw new ArgumentException(
                $"CEP inválido: {string.Join(", ", cepResult.Notifications.Select(n => n.Mensagem))}",
                nameof(dto));

        if (await _repoFactory().CepJaExiste(cepResult.Value!, dto.Id, cancellationToken))
            throw new InvalidOperationException($"Já existe outro logradouro cadastrado com o CEP {dto.Cep}.");

        var atualizado = await _repoFactory().Atualizar(existente.UpdateFromDto(dto), cancellationToken);
        return atualizado.ToDto();
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _repoFactory().ObterPorId(id, cancellationToken);
        if (existente == null) return false;

        return await _repoFactory().Remover(id, cancellationToken);
    }
}
