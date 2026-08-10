// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Entities;

public sealed class Logradouro : Entity
{
    public Cep Cep { get; private set; }
    public string Nome { get; private set; }
    public string Bairro { get; private set; }
    public string Cidade { get; private set; }
    public string Estado { get; private set; }
    public string Pais { get; private set; }

    private Logradouro(int id, Cep cep, string nome, string bairro, string cidade, string estado, string pais) : base(id)
    {
        Cep = cep;
        Nome = nome;
        Bairro = bairro;
        Cidade = cidade;
        Estado = estado;
        Pais = pais;
    }

    public static Result<Logradouro> Criar(int id, Cep? cep, string? nome, string? bairro, string? cidade, string? estado, string? pais)
    {
        var notifications = new List<Notification>();

        if (cep == null) notifications.Add(new Notification("Logradouro.Cep", "O CEP é obrigatório."));

        var nomeNorm = NormalizadoService.LimparEspacos(nome);
        if (string.IsNullOrWhiteSpace(nomeNorm)) notifications.Add(new Notification("Logradouro.Nome", "O nome do logradouro é obrigatório."));

        var bairroNorm = NormalizadoService.LimparEspacos(bairro);
        if (string.IsNullOrWhiteSpace(bairroNorm)) notifications.Add(new Notification("Logradouro.Bairro", "O bairro é obrigatório."));

        var cidadeNorm = NormalizadoService.LimparEspacos(cidade);
        if (string.IsNullOrWhiteSpace(cidadeNorm)) notifications.Add(new Notification("Logradouro.Cidade", "A cidade é obrigatória."));

        var estadoNorm = NormalizadoService.ParaMaiusculo(NormalizadoService.LimparEspacos(estado));
        if (string.IsNullOrWhiteSpace(estadoNorm) || estadoNorm.Length != 2) notifications.Add(new Notification("Logradouro.Estado", "O estado deve conter exatamente 2 letras (sigla)."));

        var paisNorm = NormalizadoService.LimparEspacos(pais);
        if (string.IsNullOrWhiteSpace(paisNorm)) notifications.Add(new Notification("Logradouro.Pais", "O país é obrigatório."));

        if (notifications.Count > 0)
            return Result.Failure<Logradouro>(notifications);

        return Result.Success(new Logradouro(id, cep!, nomeNorm, bairroNorm, cidadeNorm, estadoNorm, paisNorm));
    }
}