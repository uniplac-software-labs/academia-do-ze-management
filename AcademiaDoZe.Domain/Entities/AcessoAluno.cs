// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; protected set; }
    public DateTime Entrada { get; protected set; }
    public DateTime? Saida { get; protected set; }

    public AcessoAluno(
        int id,
        Aluno aluno,
        DateTime entrada,
        DateTime? saida)
        : base(id)
    {
        Aluno = aluno;
        Entrada = entrada;
        Saida = saida;
    }
}