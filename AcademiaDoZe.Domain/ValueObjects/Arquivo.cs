// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public string Nome { get; init; }
    public string Caminho { get; init; }

    public Arquivo(string nome, string caminho)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do arquivo é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(caminho))
            throw new ArgumentException("O caminho do arquivo é obrigatório.", nameof(caminho));

        Nome = nome;
        Caminho = caminho;
    }
}