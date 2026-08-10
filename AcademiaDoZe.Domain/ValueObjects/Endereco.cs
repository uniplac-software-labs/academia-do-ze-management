// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Services;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public Logradouro Logradouro { get; }
    public string Numero { get; }
    public string Complemento { get; }

    private Endereco(Logradouro logradouro, string numero, string complemento)
    {
        Logradouro = logradouro;
        Numero = numero;
        Complemento = complemento;
    }

    public static Result<Endereco> Criar(Logradouro? logradouro, string? numero, string? complemento)
    {
        var notifications = new List<Notification>();

        if (logradouro == null)
            notifications.Add(new Notification("Endereco.Logradouro", "O logradouro é obrigatório."));

        var numeroNorm = NormalizadoService.LimparEspacos(numero);
        if (string.IsNullOrWhiteSpace(numeroNorm))
            notifications.Add(new Notification("Endereco.Numero", "O número é obrigatório."));

        var complementoNorm = NormalizadoService.LimparEspacos(complemento);

        if (notifications.Count > 0)
            return Result.Failure<Endereco>(notifications);

        return Result.Success(new Endereco(logradouro!, numeroNorm, complementoNorm));
    }
}