namespace AcademiaDoZe.Application.DTOs;

/// <summary>Um registro de passagem na catraca (entrada ou saída) já "traduzido" para exibição.</summary>
public class AcessoDto
{
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public required string Nome { get; set; }
    public required string TipoPessoa { get; set; }
    public required string Tipo { get; set; }
    public DateTime DataHora { get; set; }
    public required string Descricao { get; set; }
    public ArquivoDto? Foto { get; set; }
}
