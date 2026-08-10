// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; }

    private Cep(string valor)
    {
        Valor = valor;
    }

    public static Result<Cep> Criar(string? valor)
    {
        var notifications = new List<Notification>();
        var valorNormalizado = NormalizadoService.LimparEDigitos(valor);

        if (string.IsNullOrWhiteSpace(valorNormalizado) || valorNormalizado.Length != 8)
        {
            notifications.Add(new Notification("Cep", "O CEP deve possuir exatamente 8 dígitos numéricos."));
            return Result.Failure<Cep>(notifications);
        }

        return Result.Success(new Cep(valorNormalizado));
    }
}