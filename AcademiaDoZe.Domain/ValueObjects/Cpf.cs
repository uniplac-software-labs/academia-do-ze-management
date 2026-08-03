// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; init; }

    public Cpf(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O CPF é obrigatório.", nameof(valor));

        Valor = valor;
    }
}