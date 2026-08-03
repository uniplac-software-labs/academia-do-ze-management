// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; init; }

    public Senha(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("A senha é obrigatória.", nameof(valor));

        Valor = valor;
    }
}