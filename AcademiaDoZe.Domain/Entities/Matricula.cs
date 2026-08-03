// Pedro Henrique dos Santos

using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity
{
    public Aluno Aluno { get; protected set; }
    public MatriculaPlano Plano { get; protected set; }
    public DateOnly DataInicio { get; protected set; }
    public DateOnly DataFinal { get; protected set; }
    public string Objetivo { get; protected set; }
    public MatriculaRestricoes Restricoes { get; protected set; }
    public string ObservacoesRestricoes { get; protected set; }
    public Arquivo? LaudoMedico { get; protected set; }

    public Matricula(
        int id,
        Aluno aluno,
        MatriculaPlano plano,
        DateOnly dataInicio,
        DateOnly dataFinal,
        string objetivo,
        MatriculaRestricoes restricoes,
        string observacoesRestricoes,
        Arquivo? laudoMedico)
        : base(id)
    {
        Aluno = aluno;
        Plano = plano;
        DataInicio = dataInicio;
        DataFinal = dataFinal;
        Objetivo = objetivo;
        Restricoes = restricoes;
        ObservacoesRestricoes = observacoesRestricoes;
        LaudoMedico = laudoMedico;
    }
}