// Nome: [PEdro Henrique dos santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Services;
using AcademiaDoZe.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity
{
    public Aluno AlunoMatricula { get; private set; }
    public MatriculaPlano Plano { get; private set; }
    public DateOnly DataInicio { get; private set; }
    public DateOnly DataFim { get; private set; }
    public string Objetivo { get; private set; }
    public MatriculaRestricoes RestricoesMedicas { get; private set; }
    public string ObservacoesRestricoes { get; private set; }
    public Arquivo? LaudoMedico { get; private set; }

    private Matricula(
        int id,
        Aluno alunoMatricula,
        MatriculaPlano plano,
        DateOnly dataInicio,
        DateOnly dataFim,
        string objetivo,
        MatriculaRestricoes restricoesMedicas,
        Arquivo? laudoMedico,
        string observacoesRestricoes = "") : base(id)
    {
        AlunoMatricula = alunoMatricula;
        Plano = plano;
        DataInicio = dataInicio;
        DataFim = dataFim;
        Objetivo = objetivo;
        RestricoesMedicas = restricoesMedicas;
        LaudoMedico = laudoMedico;
        ObservacoesRestricoes = observacoesRestricoes;
    }

    public static Result<Matricula> Criar(
        int id,
        Aluno? aluno,
        MatriculaPlano plano,
        DateOnly dataInicio,
        DateOnly dataFim,
        string? objetivo,
        MatriculaRestricoes restricoesMedicas,
        Arquivo? laudoMedico,
        string? observacoesRestricoes = "")
    {
        var notifications = new List<Notification>();

        if (aluno == null)
        {
            notifications.Add(new Notification("Matricula.Aluno", "O aluno é obrigatório."));
            return Result.Failure<Matricula>(notifications);
        }

        if (dataFim <= dataInicio)
            notifications.Add(new Notification("Matricula.DataFim", "A data final da matrícula deve ser posterior à data inicial."));

        var objetivoNorm = NormalizadoService.LimparEspacos(objetivo);
        var obsNorm = NormalizadoService.LimparEspacos(observacoesRestricoes);

        // Regra do Domínio: Cálculo da Idade do Aluno
        int idade = DateOnly.FromDateTime(DateTime.Today).Year - aluno.DataNascimento.Year;
        if (aluno.DataNascimento > DateOnly.FromDateTime(DateTime.Today).AddYears(-idade)) idade--;

        // Regra do Domínio: Alunos entre 12 e 16 anos precisam obrigatoriamente de Laudo Médico
        if (idade >= 12 && idade <= 16 && laudoMedico == null)
        {
            notifications.Add(new Notification("Matricula.LaudoMedico", "Alunos de 12 a 16 anos exigem laudo médico obrigatório."));
        }

        // Regra do Domínio: Alunos com restrições médicas registradas devem obrigatoriamente apresentar laudo/parecer médico
        if (restricoesMedicas != MatriculaRestricoes.None && laudoMedico == null)
        {
            notifications.Add(new Notification("Matricula.LaudoMedico", "Alunos com restrições de saúde exigem parecer/laudo médico obrigatório."));
        }

        if (notifications.Count > 0)
            return Result.Failure<Matricula>(notifications);

        return Result.Success(new Matricula(id, aluno, plano, dataInicio, dataFim, objetivoNorm, restricoesMedicas, laudoMedico, obsNorm));
    }
}