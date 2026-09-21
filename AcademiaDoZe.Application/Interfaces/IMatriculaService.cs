using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;

namespace AcademiaDoZe.Application.Interfaces;

public interface IMatriculaService
{
    Task<MatriculaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterTodasAsync(CancellationToken cancellationToken = default);
    Task<MatriculaDto> AdicionarAsync(MatriculaDto matriculaDto, CancellationToken cancellationToken = default);
    Task<MatriculaDto> AtualizarAsync(MatriculaDto matriculaDto, CancellationToken cancellationToken = default);
    Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterPorAlunoIdAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<MatriculaDto?> ObterMatriculaAtivaPorAlunoAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<bool> PossuiMatriculaAtivaAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterAtivasAsync(int alunoId = 0, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterVencendoEmDiasAsync(int dias, CancellationToken cancellationToken = default);
    Task<IEnumerable<MatriculaDto>> ObterPorPlanoAsync(AppMatriculaPlano plano, CancellationToken cancellationToken = default);
}
