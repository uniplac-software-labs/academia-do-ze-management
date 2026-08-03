// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Valor { get; init; }

    public Telefone(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O telefone é obrigatório.", nameof(valor));

        Valor = valor;
    }
}