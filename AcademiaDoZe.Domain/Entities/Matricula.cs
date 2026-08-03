// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.Entities;

public class Matricula : Entity
{
    public Aluno Aluno { get; protected set; }
    public DateOnly DataInicio { get; protected set; }
    public DateOnly? DataFim { get; protected set; }

    public bool Ativa => DataFim == null;

    public Matricula(
        int id,
        Aluno aluno,
        DateOnly dataInicio,
        DateOnly? dataFim = null)
        : base(id)
    {
        Aluno = aluno ?? throw new ArgumentNullException(nameof(aluno));

        if (dataInicio > DateOnly.FromDateTime(DateTime.Today))
            throw new ArgumentException(
                "A data de início da matrícula não pode estar no futuro.",
                nameof(dataInicio));

        if (dataFim.HasValue && dataFim < dataInicio)
            throw new ArgumentException(
                "A data de fim não pode ser anterior à data de início.",
                nameof(dataFim));

        DataInicio = dataInicio;
        DataFim = dataFim;
    }
}