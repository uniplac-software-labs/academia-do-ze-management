// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.Entities;

public class AcessoAluno : Entity
{
    public Aluno Aluno { get; protected set; }
    public DateTime DataAcesso { get; protected set; }

    public AcessoAluno(
        int id,
        Aluno aluno,
        DateTime dataAcesso)
        : base(id)
    {
        Aluno = aluno ?? throw new ArgumentNullException(nameof(aluno));

        if (dataAcesso > DateTime.Now)
            throw new ArgumentException(
                "A data de acesso não pode estar no futuro.",
                nameof(dataAcesso));

        DataAcesso = dataAcesso;
    }
}