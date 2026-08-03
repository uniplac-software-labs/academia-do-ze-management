// Pedro Henrique dos Santos

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Logradouro : Entity
{
    public Cep Cep { get; protected set; }
    public string Pais { get; protected set; }
    public string Estado { get; protected set; }
    public string Cidade { get; protected set; }
    public string Bairro { get; protected set; }
    public string Nome { get; protected set; }

    public Logradouro(
        int id,
        Cep cep,
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nome)
        : base(id)
    {
        Cep = cep ?? throw new ArgumentNullException(nameof(cep));

        if (string.IsNullOrWhiteSpace(pais))
            throw new ArgumentException("O país é obrigatório.", nameof(pais));

        if (string.IsNullOrWhiteSpace(estado))
            throw new ArgumentException("O estado é obrigatório.", nameof(estado));

        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("A cidade é obrigatória.", nameof(cidade));

        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("O bairro é obrigatório.", nameof(bairro));

        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do logradouro é obrigatório.", nameof(nome));

        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        Nome = nome;
    }
}