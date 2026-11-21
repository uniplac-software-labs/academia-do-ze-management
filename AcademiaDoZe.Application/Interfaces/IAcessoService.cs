using AcademiaDoZe.Application.DTOs;

namespace AcademiaDoZe.Application.Interfaces;

public interface IAcessoService
{
    /// <summary>Registra entrada ou saída do aluno (alterna conforme o estado atual) aplicando as regras de negócio.</summary>
    Task<AcessoResultadoDto> RegistrarAcessoAlunoAsync(int alunoId, CancellationToken cancellationToken = default);

    /// <summary>Registra entrada ou saída do colaborador (alterna conforme o estado atual) aplicando as regras de negócio.</summary>
    Task<AcessoResultadoDto> RegistrarAcessoColaboradorAsync(int colaboradorId, CancellationToken cancellationToken = default);

    Task<bool> AlunoEstaNaAcademiaAsync(int alunoId, CancellationToken cancellationToken = default);
    Task<bool> ColaboradorEstaNaAcademiaAsync(int colaboradorId, CancellationToken cancellationToken = default);

    /// <summary>Acessos (alunos e colaboradores) registrados hoje, do mais recente para o mais antigo.</summary>
    Task<IEnumerable<AcessoDto>> ObterAcessosDeHojeAsync(CancellationToken cancellationToken = default);
}
