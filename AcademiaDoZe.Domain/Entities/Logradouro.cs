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
        Cep = cep;
        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        Nome = nome;
    }
}