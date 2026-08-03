// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; init; }

    public Senha(string valor)
    {
        Valor = valor;
    }
}