namespace AcademiaDoZe.Application.DTOs;

/// <summary>Resultado de uma tentativa de passar na catraca: liberou ou negou, e a mensagem a mostrar.</summary>
public class AcessoResultadoDto
{
    public bool Autorizado { get; set; }
    public required string Titulo { get; set; }
    public required string Mensagem { get; set; }
    public bool EhEntrada { get; set; }
    public DateTime DataHora { get; set; }
}
