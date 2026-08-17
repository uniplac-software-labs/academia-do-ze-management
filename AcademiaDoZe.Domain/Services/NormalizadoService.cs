// Pedro Henrique dos Santos
using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Services;

public static partial class NormalizacaoService
{
    public static bool TextoVazioOuNulo(string? texto) => string.IsNullOrWhiteSpace(texto);
    public static string LimparEspacos(string? texto) => string.IsNullOrWhiteSpace(texto) ? string.Empty : EspacosRegex().Replace(texto, " ").Trim();
    public static string LimparTodosEspacos(string? texto) => string.IsNullOrWhiteSpace(texto) ? string.Empty : texto.Replace(" ", string.Empty);
    public static string ParaMaiusculo(string? texto) => string.IsNullOrEmpty(texto) ? string.Empty : texto.ToUpperInvariant();
    public static string LimparEDigitos(string? texto) => string.IsNullOrEmpty(texto) ? string.Empty : new string([.. texto.Where(char.IsDigit)]);

    [GeneratedRegex(@"\s+")]
    private static partial Regex EspacosRegex();
}