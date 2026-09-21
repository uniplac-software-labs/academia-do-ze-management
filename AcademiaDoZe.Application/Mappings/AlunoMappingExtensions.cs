using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class AlunoMappingExtensions
{
    public static AlunoDto ToDto(this Aluno aluno, Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(aluno);

        return new AlunoDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf.Valor,
            DataNascimento = aluno.DataNascimento,
            Telefone = aluno.Telefone.Valor,
            Email = aluno.Email?.Valor,
            Endereco = logradouro?.ToDto(),
            Numero = aluno.Endereco?.Numero ?? string.Empty,
            Complemento = aluno.Endereco?.Complemento,
            Senha = null,
            Foto = aluno.Foto?.Conteudo != null
                ? new ArquivoDto { Conteudo = aluno.Foto.Conteudo }
                : null
        };
    }

    public static Aluno ToEntity(this AlunoDto dto, Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var logradouroEntidade =
            logradouro ??
            (dto.Endereco != null ? dto.Endereco.ToEntity() : null) ??
            throw new InvalidOperationException("Logradouro/Endereço é obrigatório para converter o Aluno.");

        Arquivo? foto = null;

        if (dto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(dto.Foto.Conteudo);
            if (fotoResult.IsFailure)
                throw new InvalidOperationException(
                    $"Foto inválida: {string.Join(", ", fotoResult.Notifications.Select(n => n.Mensagem))}");
            foto = fotoResult.Value;
        }

        var result = Aluno.Criar(
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
            foto!);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao converter Aluno: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }

    public static Aluno UpdateFromDto(this Aluno aluno, AlunoDto dto, Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(aluno);
        ArgumentNullException.ThrowIfNull(dto);

        var logradouroEntidade =
            logradouro ??
            (dto.Endereco != null ? dto.Endereco.ToEntity() : null) ??
            throw new InvalidOperationException("Logradouro/Endereço é obrigatório para atualizar o Aluno.");

        Arquivo? foto = aluno.Foto;

        if (dto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(dto.Foto.Conteudo);
            if (fotoResult.IsFailure)
                throw new InvalidOperationException(
                    $"Foto inválida: {string.Join(", ", fotoResult.Notifications.Select(n => n.Mensagem))}");
            foto = fotoResult.Value;
        }

        string senha = !string.IsNullOrWhiteSpace(dto.Senha) ? dto.Senha : aluno.Senha.Valor;

        var result = Aluno.Criar(
            aluno.Id,
            dto.Nome,
            aluno.Cpf.Valor,
            dto.DataNascimento,
            dto.Telefone,
            dto.Email ?? aluno.Email.Valor,
            logradouroEntidade,
            dto.Numero,
            dto.Complemento ?? aluno.Endereco.Complemento,
            senha,
            foto!);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Erro de validação ao atualizar Aluno: {string.Join(", ", result.Notifications.Select(n => n.Mensagem))}");

        return result.Value!;
    }
}
