// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity
{
    public Colaborador Colaborador { get; protected set; }
    public DateTime DataAcesso { get; protected set; }

    public AcessoColaborador(
        int id,
        Colaborador colaborador,
        DateTime dataAcesso)
        : base(id)
    {
        Colaborador = colaborador ?? throw new ArgumentNullException(nameof(colaborador));

        if (dataAcesso > DateTime.Now)
            throw new ArgumentException(
                "A data de acesso não pode estar no futuro.",
                nameof(dataAcesso));

        DataAcesso = dataAcesso;
    }
}