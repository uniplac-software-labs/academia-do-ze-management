using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services;

public class MatriculaService : IMatriculaService
{
    private readonly Func<IMatriculaRepository> _matriculaRepoFactory;
    private readonly Func<IAlunoRepository> _alunoRepoFactory;

    public MatriculaService(
        Func<IMatriculaRepository> matriculaRepoFactory,
        Func<IAlunoRepository> alunoRepoFactory)
    {
        _matriculaRepoFactory = matriculaRepoFactory ?? throw new ArgumentNullException(nameof(matriculaRepoFactory));
        _alunoRepoFactory = alunoRepoFactory ?? throw new ArgumentNullException(nameof(alunoRepoFactory));
    }

    public async Task<MatriculaDto?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var matricula = await _matriculaRepoFactory().ObterPorId(id, cancellationToken);
        if (matricula == null) return null;

        var aluno = await _alunoRepoFactory().ObterPorId(matricula.AlunoId, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno associado à matrícula {id} não encontrado.");

        return matricula.ToDto(aluno.ToDto());
    }

    public async Task<IEnumerable<MatriculaDto>> ObterTodasAsync(CancellationToken cancellationToken = default)
    {
        var matriculas = await _matriculaRepoFactory().ObterTodos(cancellationToken);
        return await EnriquecerComAlunosAsync(matriculas, cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorAlunoIdAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var aluno = await _alunoRepoFactory().ObterPorId(alunoId, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

        var matriculas = await _matriculaRepoFactory().ObterPorAluno(alunoId, cancellationToken);
        var alunoDto = aluno.ToDto();

        return [.. matriculas.Select(m => m.ToDto(alunoDto))];
    }

    public async Task<MatriculaDto?> ObterMatriculaAtivaPorAlunoAsync(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var matricula = await _matriculaRepoFactory().ObterMatriculaAtivaPorAluno(alunoId, cancellationToken);
        if (matricula == null) return null;

        var aluno = await _alunoRepoFactory().ObterPorId(alunoId, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno com ID {alunoId} não encontrado.");

        return matricula.ToDto(aluno.ToDto());
    }

    public Task<bool> PossuiMatriculaAtivaAsync(int alunoId, CancellationToken cancellationToken = default)
        => _matriculaRepoFactory().PossuiMatriculaAtiva(alunoId, cancellationToken);

    public async Task<IEnumerable<MatriculaDto>> ObterAtivasAsync(
        int alunoId = 0,
        CancellationToken cancellationToken = default)
    {
        var matriculas = await _matriculaRepoFactory().ObterAtivas(alunoId, cancellationToken);
        return await EnriquecerComAlunosAsync(matriculas, cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterVencendoEmDiasAsync(
        int dias,
        CancellationToken cancellationToken = default)
    {
        if (dias < 0)
            throw new ArgumentOutOfRangeException(nameof(dias), "Dias não pode ser negativo.");

        var matriculas = await _matriculaRepoFactory().ObterVencendoEmDias(dias, cancellationToken);
        return await EnriquecerComAlunosAsync(matriculas, cancellationToken);
    }

    public async Task<IEnumerable<MatriculaDto>> ObterPorPlanoAsync(
        AppMatriculaPlano plano,
        CancellationToken cancellationToken = default)
    {
        var matriculas = await _matriculaRepoFactory().ObterPorPlano(plano.ToDomain(), cancellationToken);
        return await EnriquecerComAlunosAsync(matriculas, cancellationToken);
    }

    public async Task<MatriculaDto> AdicionarAsync(
        MatriculaDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.AlunoMatricula == null || dto.AlunoMatricula.Id <= 0)
            throw new InvalidOperationException("Aluno não informado ou com ID inválido para matrícula.");

        var aluno = await _alunoRepoFactory().ObterPorId(dto.AlunoMatricula.Id, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno com ID {dto.AlunoMatricula.Id} não encontrado.");

        if (await _matriculaRepoFactory().PossuiMatriculaAtiva(aluno.Id, cancellationToken))
            throw new InvalidOperationException("Já existe uma matrícula ativa para este aluno.");

        bool menorDe16 = aluno.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16));
        bool possuiLaudo = dto.LaudoMedico?.Conteudo is { Length: > 0 };

        if (menorDe16 && !possuiLaudo)
            throw new InvalidOperationException(
                "Alunos menores de 16 anos devem obrigatoriamente apresentar um laudo médico que os autorize a praticar atividades físicas.");

        if (dto.RestricoesMedicas != AppMatriculaRestricoes.None && !possuiLaudo)
            throw new InvalidOperationException(
                "Alunos com restrições de saúde registradas devem apresentar um parecer médico autorizando a realização de atividades físicas.");

        var adicionada = await _matriculaRepoFactory().Adicionar(dto.ToEntity(aluno), cancellationToken);
        return adicionada.ToDto(aluno.ToDto());
    }

    public async Task<MatriculaDto> AtualizarAsync(
        MatriculaDto dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var existente = await _matriculaRepoFactory().ObterPorId(dto.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Matrícula com ID {dto.Id} não encontrada.");

        var aluno = await _alunoRepoFactory().ObterPorId(existente.AlunoId, cancellationToken)
            ?? throw new InvalidOperationException($"Aluno associado à matrícula {dto.Id} não encontrado.");

        bool menorDe16 = aluno.DataNascimento > DateOnly.FromDateTime(DateTime.Today.AddYears(-16));
        bool possuiLaudo =
            dto.LaudoMedico?.Conteudo is { Length: > 0 } ||
            (dto.LaudoMedico == null && existente.LaudoMedico != null);

        if (menorDe16 && !possuiLaudo)
            throw new InvalidOperationException(
                "Alunos menores de 16 anos devem obrigatoriamente apresentar um laudo médico que os autorize a praticar atividades físicas.");

        if (dto.RestricoesMedicas != AppMatriculaRestricoes.None && !possuiLaudo)
            throw new InvalidOperationException(
                "Alunos com restrições de saúde registradas devem apresentar um parecer médico autorizando a realização de atividades físicas.");

        var atualizada = await _matriculaRepoFactory().Atualizar(existente.UpdateFromDto(dto, aluno), cancellationToken);
        return atualizada.ToDto(aluno.ToDto());
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var existente = await _matriculaRepoFactory().ObterPorId(id, cancellationToken);
        if (existente == null) return false;

        return await _matriculaRepoFactory().Remover(id, cancellationToken);
    }

    private async Task<IEnumerable<MatriculaDto>> EnriquecerComAlunosAsync(
        IEnumerable<AcademiaDoZe.Domain.Entities.Matricula> matriculas,
        CancellationToken cancellationToken)
    {
        var lista = matriculas.ToList();
        if (lista.Count == 0) return [];

        var alunos = new Dictionary<int, AlunoDto>();

        foreach (var id in lista.Select(m => m.AlunoId).Distinct())
        {
            var aluno = await _alunoRepoFactory().ObterPorId(id, cancellationToken);
            if (aluno != null)
                alunos[id] = aluno.ToDto();
        }

        return [.. lista.Select(m =>
        {
            if (!alunos.TryGetValue(m.AlunoId, out var alunoDto))
                throw new InvalidOperationException($"Aluno associado à matrícula {m.Id} não encontrado.");

            return m.ToDto(alunoDto);
        })];
    }
}
