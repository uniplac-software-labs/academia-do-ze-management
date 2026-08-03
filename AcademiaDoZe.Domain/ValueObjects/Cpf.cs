// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; init; }

    public Cpf(string valor)
    {
        Valor = valor;
    }
}