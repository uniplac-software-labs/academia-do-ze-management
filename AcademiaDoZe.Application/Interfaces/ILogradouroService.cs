using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface ILogradouroService
{
    Task<LogradouroDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LogradouroDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<LogradouroDto> AdicionarAsync(LogradouroDto logradouroDto, CancellationToken cancellationToken = default);
    Task<LogradouroDto> AtualizarAsync(LogradouroDto logradouroDto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<LogradouroDto?> ObterPorCepAsync(string cep, CancellationToken cancellationToken = default);
    Task<bool> CepJaExisteAsync(string cep, int? id = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<LogradouroDto>> ObterPorCidadeAsync(string cidade, CancellationToken cancellationToken = default);
    Task<IEnumerable<LogradouroDto>> ObterPorBairroAsync(string cidade, string bairro, CancellationToken cancellationToken = default);
}
