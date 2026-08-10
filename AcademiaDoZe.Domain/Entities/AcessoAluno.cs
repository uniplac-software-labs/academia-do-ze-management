// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using System;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; private set; }
    public DateTime DataHora { get; private set; }

    private AcessoAluno(int id, Aluno aluno, DateTime dataHora) : base(id)
    {
        Aluno = aluno;
        DataHora = dataHora;
    }

    public static Result<AcessoAluno> Criar(int id, Aluno? aluno, DateTime dataHora)
    {
        if (aluno == null)
            return Result.Failure<AcessoAluno>("AcessoAluno.Aluno", "O aluno é obrigatório para registrar o acesso.");

        return Result.Success(new AcessoAluno(id, aluno, dataHora));
    }
}