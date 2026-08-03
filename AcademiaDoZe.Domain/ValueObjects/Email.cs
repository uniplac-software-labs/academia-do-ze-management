// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; init; }

    public Email(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O e-mail é obrigatório.", nameof(valor));

        Valor = valor;
    }
}