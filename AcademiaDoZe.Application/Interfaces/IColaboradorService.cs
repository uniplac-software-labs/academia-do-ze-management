using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;

namespace AcademiaDoZe.Application.Interfaces;

public interface IColaboradorService
{
    Task<ColaboradorDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ColaboradorDto>> ObterTodosAsync(CancellationToken cancellationToken = default);
    Task<ColaboradorDto> AdicionarAsync(ColaboradorDto colaboradorDto, CancellationToken cancellationToken = default);
    Task<ColaboradorDto> AtualizarAsync(ColaboradorDto colaboradorDto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<ColaboradorDto?> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<ColaboradorDto?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ColaboradorDto>> ObterPorTipoAsync(AppColaboradorTipo tipo, CancellationToken cancellationToken = default);
    Task<IEnumerable<ColaboradorDto>> ObterPorVinculoAsync(AppColaboradorVinculo vinculo, CancellationToken cancellationToken = default);
    Task<bool> CpfJaExisteAsync(string cpf, int? id = null, CancellationToken cancellationToken = default);
    Task<bool> EmailJaExisteAsync(string email, int? id = null, CancellationToken cancellationToken = default);
    Task<bool> TrocarSenhaAsync(int id, string novaSenha, CancellationToken cancellationToken = default);
}
