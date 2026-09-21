using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class ColaboradorMappingExtensions
{
    public static ColaboradorDto ToDto(this Colaborador colaborador, Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(colaborador);

        return new ColaboradorDto
        {
            Id = colaborador.Id,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf.Valor,
            DataNascimento = colaborador.DataNascimento,
            Telefone = colaborador.Telefone.Valor,
            Email = colaborador.Email?.Valor,
            Endereco = logradouro?.ToDto(),
            Numero = colaborador.Endereco?.Numero ?? string.Empty,
            Complemento = colaborador.Endereco?.Complemento,
            Senha = null,
            Foto = colaborador.Foto?.Conteudo != null
                ? new ArquivoDto { Conteudo = colaborador.Foto.Conteudo }
                : null,
            DataAdmissao = colaborador.DataAdmissao,
            Tipo = colaborador.Tipo.ToApplication(),
            Vinculo = colaborador.Vinculo.ToApplication()
        };
    }

    public static Colaborador ToEntity(this ColaboradorDto dto, Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var logradouroEntidade =
            logradouro ??
            (dto.Endereco != null ? dto.Endereco.ToEntity() : null) ??
            throw new InvalidOperationException("Logradouro/Endereço é obrigatório para converter o Colaborador.");

        Arquivo? foto = null;

        if (dto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(dto.Foto.Conteudo);
            if (fotoResult.IsFailure)
                throw new InvalidOperationException(
                    $"Foto inválida: {string.Join(", ", fotoResult.Notifications.Select(n => n.Mensagem))}");
            foto = fotoResult.Value;
        }

        var result = Colaborador.Criar(
            dto.Id,
            dto.Nome,
            dto.Cpf,
            dto.DataNascimento,
            dto.Telefone,
            dto.Email ?? string.Empty,
            logradouroEntidade,
            dto.Numero,
            dto.Complemento ?? string.Empty,
            dto.Senha ?? string.Empty,
            foto!,
            dto.DataAdmissao,
            dto.Tipo.ToDomain(),
            dto.Vinculo.ToDomain());

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao converter Colaborador: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }

    public static Colaborador UpdateFromDto(this Colaborador colaborador, ColaboradorDto dto, Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(colaborador);
        ArgumentNullException.ThrowIfNull(dto);

        var logradouroEntidade =
            logradouro ??
            (dto.Endereco != null ? dto.Endereco.ToEntity() : null) ??
            throw new InvalidOperationException("Logradouro/Endereço é obrigatório para atualizar o Colaborador.");

        Arquivo? foto = colaborador.Foto;

        if (dto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(dto.Foto.Conteudo);
            if (fotoResult.IsFailure)
                throw new InvalidOperationException(
                    $"Foto inválida: {string.Join(", ", fotoResult.Notifications.Select(n => n.Mensagem))}");
            foto = fotoResult.Value;
        }

        string senha = !string.IsNullOrWhiteSpace(dto.Senha) ? dto.Senha : colaborador.Senha.Valor;

        var result = Colaborador.Criar(
            colaborador.Id,
            dto.Nome,
            colaborador.Cpf.Valor,
            dto.DataNascimento,
            dto.Telefone,
            dto.Email ?? colaborador.Email.Valor,
            logradouroEntidade,
            dto.Numero,
            dto.Complemento ?? colaborador.Endereco.Complemento,
            senha,
            foto!,
            dto.DataAdmissao,
            dto.Tipo.ToDomain(),
            dto.Vinculo.ToDomain());

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao atualizar Colaborador: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }
}
