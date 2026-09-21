using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Application.Mappings;

public static class LogradouroMappingExtensions
{
    public static LogradouroDto ToDto(this Logradouro logradouro)
    {
        ArgumentNullException.ThrowIfNull(logradouro);

        return new LogradouroDto
        {
            Id = logradouro.Id,
            Cep = logradouro.Cep.Valor,
            Nome = logradouro.Nome,
            Bairro = logradouro.Bairro,
            Cidade = logradouro.Cidade,
            Estado = logradouro.Estado,
            Pais = logradouro.Pais
        };
    }

    public static Logradouro ToEntity(this LogradouroDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var result = Logradouro.Criar(dto.Id, dto.Cep, dto.Nome, dto.Bairro, dto.Cidade, dto.Estado, dto.Pais);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao converter Logradouro: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }

    public static Logradouro UpdateFromDto(this Logradouro logradouro, LogradouroDto dto)
    {
        ArgumentNullException.ThrowIfNull(logradouro);
        ArgumentNullException.ThrowIfNull(dto);

        var result = Logradouro.Criar(
            logradouro.Id,
            dto.Cep,
            dto.Nome,
            dto.Bairro,
            dto.Cidade,
            dto.Estado,
            dto.Pais);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao atualizar Logradouro: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }
}
