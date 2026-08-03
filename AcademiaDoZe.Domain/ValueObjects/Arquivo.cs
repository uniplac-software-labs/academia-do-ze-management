// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public string Nome { get; init; }
    public string Caminho { get; init; }

    public Arquivo(string nome, string caminho)
    {
        Nome = nome;
        Caminho = caminho;
    }
}