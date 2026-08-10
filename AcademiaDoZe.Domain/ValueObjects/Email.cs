// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Result<Email> Criar(string? valor)
    {
        var notifications = new List<Notification>();
        var valorNormalizado = NormalizadoService.ParaMinusculo(NormalizadoService.LimparEspacos(valor));

        if (string.IsNullOrWhiteSpace(valorNormalizado) || !Regex.IsMatch(valorNormalizado, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            notifications.Add(new Notification("Email", "Formato de e-mail inválido."));
            return Result.Failure<Email>(notifications);
        }

        return Result.Success(new Email(valorNormalizado));
    }
}