// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Telefone
{
    public string Valor { get; init; }

    public Telefone(string valor)
    {
        Valor = valor;
    }
}