// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cep
{
    public string Valor { get; init; }

    public Cep(string valor)
    {
        Valor = valor;
    }
}