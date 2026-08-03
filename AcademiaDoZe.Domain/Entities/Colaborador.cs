// Pedro Henrique dos Santos

using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Colaborador : Pessoa
{
    public DateOnly DataAdmissao { get; protected set; }
    public ColaboradorTipo Tipo { get; protected set; }
    public ColaboradorVinculo Vinculo { get; protected set; }

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
        DateOnly dataAdmissao,
        ColaboradorTipo tipo,
        ColaboradorVinculo vinculo)
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
        DataAdmissao = dataAdmissao;
        Tipo = tipo;
        Vinculo = vinculo;
    }
}