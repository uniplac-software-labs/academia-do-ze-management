using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class MatriculaMappingExtensions
{
    public static MatriculaDto ToDto(this Matricula matricula, AlunoDto alunoDto)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(alunoDto);

        return new MatriculaDto
        {
            Id = matricula.Id,
            AlunoMatricula = alunoDto,
            Plano = matricula.Plano.ToApplication(),
            DataInicio = matricula.DataInicio,
            DataFim = matricula.DataFim,
            Objetivo = matricula.Objetivo,
            RestricoesMedicas = matricula.RestricoesMedicas.ToApplication(),
            ObservacoesRestricoes = matricula.ObservacoesRestricoes,
            LaudoMedico = matricula.LaudoMedico != null
                ? new ArquivoDto { Conteudo = matricula.LaudoMedico.Conteudo }
                : null
        };
    }

    public static Matricula ToEntity(this MatriculaDto dto, Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(aluno);

        Arquivo? laudo = null;

        if (dto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult = Arquivo.Criar(dto.LaudoMedico.Conteudo);
            if (laudoResult.IsFailure)
                throw new InvalidOperationException(
                    $"Laudo médico inválido: {string.Join(", ", laudoResult.Notifications.Select(n => n.Mensagem))}");
            laudo = laudoResult.Value;
        }

        var result = Matricula.Criar(
            dto.Id,
            aluno,
            dto.Plano.ToDomain(),
            dto.DataInicio,
            dto.Objetivo,
            dto.RestricoesMedicas.ToDomain(),
            laudo,
            dto.ObservacoesRestricoes ?? string.Empty);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao converter Matrícula: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }

    public static Matricula UpdateFromDto(this Matricula matricula, MatriculaDto dto, Aluno aluno)
    {
        ArgumentNullException.ThrowIfNull(matricula);
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(aluno);

        Arquivo? laudo = matricula.LaudoMedico;

        if (dto.LaudoMedico?.Conteudo != null)
        {
            var laudoResult = Arquivo.Criar(dto.LaudoMedico.Conteudo);
            if (laudoResult.IsFailure)
                throw new InvalidOperationException(
                    $"Laudo médico inválido: {string.Join(", ", laudoResult.Notifications.Select(n => n.Mensagem))}");
            laudo = laudoResult.Value;
        }

        var result = Matricula.Criar(
            matricula.Id,
            aluno,
            dto.Plano.ToDomain(),
            dto.DataInicio,
            dto.Objetivo,
            dto.RestricoesMedicas.ToDomain(),
            laudo,
            dto.ObservacoesRestricoes ?? matricula.ObservacoesRestricoes);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao atualizar Matrícula: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }
}
