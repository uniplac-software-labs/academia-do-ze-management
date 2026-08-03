// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; init; }

    public Email(string valor)
    {
        Valor = valor;
    }
}