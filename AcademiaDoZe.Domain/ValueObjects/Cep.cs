// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; init; }

    public Cep(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O CEP é obrigatório.", nameof(valor));

        Valor = valor;
    }
}