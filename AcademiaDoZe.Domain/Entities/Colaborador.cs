// Pedro Henrique dos Santos

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa
{
    public string Cargo { get; protected set; }
    public decimal Salario { get; protected set; }

    public Colaborador(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto,
        string cargo,
        decimal salario)
        : base(
            id,
            nome,
            cpf,
            dataNascimento,
            telefone,
            email,
            endereco,
            senha,
            foto)
    {
        if (string.IsNullOrWhiteSpace(cargo))
            throw new ArgumentException(
                "O cargo é obrigatório.",
                nameof(cargo));

        if (salario < 0)
            throw new ArgumentOutOfRangeException(
                nameof(salario),
                "O salário não pode ser negativo.");

        Cargo = cargo;
        Salario = salario;
    }
}