// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string? valor)
    {
        var notifications = new List<Notification>();

        if (string.IsNullOrWhiteSpace(valor) || valor.Length < 6)
        {
            notifications.Add(new Notification("Senha", "A senha deve possuir no mínimo 6 caracteres."));
            return Result.Failure<Senha>(notifications);
        }

        return Result.Success(new Senha(valor));
    }
}