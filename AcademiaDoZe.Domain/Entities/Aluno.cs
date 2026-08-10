// Nome: [PEdro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Entities;

public class Aluno : Pessoa
{
    private Aluno(int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Endereco endereco, Senha senha, Arquivo? foto)
        : base(id, nome, cpf, dataNascimento, telefone, email, endereco, senha, foto)
    {
    }

    public static Result<Aluno> Criar(
        int id,
        string? nome,
        Cpf? cpf,
        DateOnly dataNascimento,
        Telefone? telefone,
        Email? email,
        Endereco? endereco,
        Senha? senha,
        Arquivo? foto = null)
    {
        var listaErros = new List<Notification>();

        var nomeNorm = NormalizadoService.LimparEspacos(nome);
        if (string.IsNullOrWhiteSpace(nomeNorm))
            listaErros.Add(new Notification("Aluno.Nome", "O nome é obrigatório."));

        if (cpf == null)
            listaErros.Add(new Notification("Aluno.Cpf", "O CPF é obrigatório."));

        if (dataNascimento > DateOnly.FromDateTime(DateTime.Today))
            listaErros.Add(new Notification("Aluno.DataNascimento", "A data de nascimento não pode ser no futuro."));

        if (telefone == null)
            listaErros.Add(new Notification("Aluno.Telefone", "O telefone é obrigatório."));

        if (email == null)
            listaErros.Add(new Notification("Aluno.Email", "O e-mail é obrigatório."));

        if (endereco == null)
            listaErros.Add(new Notification("Aluno.Endereco", "O endereço é obrigatório."));

        if (senha == null)
            listaErros.Add(new Notification("Aluno.Senha", "A senha é obrigatória."));

        if (listaErros.Count > 0)
            return Result.Failure<Aluno>(listaErros);

        return Result.Success(new Aluno(id, nomeNorm, cpf!, dataNascimento, telefone!, email!, endereco!, senha!, foto));
    }
}