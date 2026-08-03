// Pedro Henrique dos Santos

namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity
{
    public Colaborador Colaborador { get; protected set; }
    public DateTime Entrada { get; protected set; }
    public DateTime? Saida { get; protected set; }

    public AcessoColaborador(
        int id,
        Colaborador colaborador,
        DateTime entrada,
        DateTime? saida)
        : base(id)
    {
        Colaborador = colaborador;
        Entrada = entrada;
        Saida = saida;
    }
}