using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAlunoService
{
    Task<AlunoDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AlunoDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<AlunoDto> AdicionarAsync(AlunoDto alunoDto, CancellationToken cancellationToken = default);
    Task<AlunoDto> AtualizarAsync(AlunoDto alunoDto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<AlunoDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<AlunoDto?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<AlunoDto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default);
    Task<bool> CpfJaExisteAsync(string cpf, int? id = null, CancellationToken cancellationToken = default);
    Task<bool> EmailJaExisteAsync(string email, int? id = null, CancellationToken cancellationToken = default);
    Task<bool> TrocarSenhaAsync(int id, string novaSenha, CancellationToken cancellationToken = default);
}
