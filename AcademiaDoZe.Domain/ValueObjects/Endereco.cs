// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.ValueObjects;

public record Endereco
{
    public Cep Cep { get; init; }
    public string Pais { get; init; }
    public string Estado { get; init; }
    public string Cidade { get; init; }
    public string Bairro { get; init; }
    public string NomeLogradouro { get; init; }
    public string Numero { get; init; }
    public string Complemento { get; init; }

    public Endereco(
        Cep cep,
        string pais,
        string estado,
        string cidade,
        string bairro,
        string nomeLogradouro,
        string numero,
        string complemento)
    {
        Cep = cep;
        Pais = pais;
        Estado = estado;
        Cidade = cidade;
        Bairro = bairro;
        NomeLogradouro = nomeLogradouro;
        Numero = numero;
        Complemento = complemento;
    }
}