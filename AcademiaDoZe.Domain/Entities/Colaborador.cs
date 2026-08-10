// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa
{
    public DateOnly DataAdmissao { get; private set; }
    public ColaboradorTipo Tipo { get; private set; }
    public ColaboradorVinculo Vinculo { get; private set; }

    private Colaborador(
        int id, string nome, Cpf cpf, DateOnly dataNascimento, Telefone telefone, Email email, Endereco endereco, Senha senha, Arquivo? foto,
        DateOnly dataAdmissao, ColaboradorTipo tipo, ColaboradorVinculo vinculo)
        : base(id, nome, cpf, dataNascimento, telefone, email, endereco, senha, foto)
    {
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }

    public static Result<Colaborador> Criar(
        int id,
        string? nome,
        Cpf? cpf,
        DateOnly dataNascimento,
        Telefone? telefone,
        Email? email,
        Endereco? endereco,
        Senha? senha,
        DateOnly dataAdmissao,
        ColaboradorTipo tipo,
        ColaboradorVinculo vinculo,
        Arquivo? foto = null)
    {
        var notifications = new List<Notification>();

        var nomeNorm = NormalizadoService.LimparEspacos(nome);
        if (string.IsNullOrWhiteSpace(nomeNorm)) notifications.Add(new Notification("Colaborador.Nome", "O nome é obrigatório."));
        if (cpf == null) notifications.Add(new Notification("Colaborador.Cpf", "O CPF é obrigatório."));
        if (dataNascimento > DateOnly.FromDateTime(DateTime.Today)) notifications.Add(new Notification("Colaborador.DataNascimento", "Data de nascimento inválida."));
        if (telefone == null) notifications.Add(new Notification("Colaborador.Telefone", "O telefone é obrigatório."));
        if (email == null) notifications.Add(new Notification("Colaborador.Email", "O e-mail é obrigatório."));
        if (endereco == null) notifications.Add(new Notification("Colaborador.Endereco", "O endereço é obrigatório."));
        if (senha == null) notifications.Add(new Notification("Colaborador.Senha", "A senha é obrigatória."));

        if (dataAdmissao > DateOnly.FromDateTime(DateTime.Today))
            notifications.Add(new Notification("Colaborador.DataAdmissao", "A data de admissão não pode ser futura."));

        if (notifications.Count > 0)
            return Result.Failure<Colaborador>(notifications);

        return Result.Success(new Colaborador(id, nomeNorm, cpf!, dataNascimento, telefone!, email!, endereco!, senha!, foto, dataAdmissao, tipo, vinculo));
    }
}