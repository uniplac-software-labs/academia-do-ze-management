// Pedro Henrique dos Santos

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa
{
    public DateOnly DataMatricula { get; protected set; }

    public Aluno(
        int id,
        string nome,
        Cpf cpf,
        DateOnly dataNascimento,
        Telefone telefone,
        Email email,
        Endereco endereco,
        Senha senha,
        Arquivo foto,
        DateOnly dataMatricula)
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
        if (dataMatricula > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException(
                "A data de matrícula não pode estar no futuro.",
                nameof(dataMatricula));

        DataMatricula = dataMatricula;
    }
}