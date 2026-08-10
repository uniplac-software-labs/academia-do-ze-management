// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public byte[] Conteudo { get; }

    private Arquivo(byte[] conteudo)
    {
        Conteudo = conteudo;
    }

    public static Result<Arquivo> Criar(byte[]? conteudo)
    {
        var notifications = new List<Notification>();

        if (conteudo == null || conteudo.Length == 0)
        {
            notifications.Add(new Notification("Arquivo", "O conteúdo do arquivo não pode ser vazio."));
            return Result.Failure<Arquivo>(notifications);
        }

        return Result.Success(new Arquivo(conteudo));
    }
}