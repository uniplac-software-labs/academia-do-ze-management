// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Valor { get; }

    private Telefone(string valor)
    {
        Valor = valor;
    }

    public static Result<Telefone> Criar(string? valor)
    {
        var notifications = new List<Notification>();
        var valorNormalizado = NormalizadoService.LimparEDigitos(valor);

        if (string.IsNullOrWhiteSpace(valorNormalizado) || valorNormalizado.Length < 10 || valorNormalizado.Length > 11)
        {
            notifications.Add(new Notification("Telefone", "O telefone deve conter entre 10 e 11 dígitos com DDD."));
            return Result.Failure<Telefone>(notifications);
        }

        return Result.Success(new Telefone(valorNormalizado));
    }
}