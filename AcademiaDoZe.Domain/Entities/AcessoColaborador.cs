// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using System;

namespace AcademiaDoZe.Domain.Entities;

public class AcessoColaborador : Entity
{
    public Colaborador Colaborador { get; private set; }
    public DateTime DataHora { get; private set; }

    private AcessoColaborador(int id, Colaborador colaborador, DateTime dataHora) : base(id)
    {
        Colaborador = colaborador;
        DataHora = dataHora;
    }

    public static Result<AcessoColaborador> Criar(int id, Colaborador? colaborador, DateTime dataHora)
    {
        if (colaborador == null)
            return Result.Failure<AcessoColaborador>("AcessoColaborador.Colaborador", "O colaborador é obrigatório para registrar o acesso.");

        return Result.Success(new AcessoColaborador(id, colaborador, dataHora));
    }
}