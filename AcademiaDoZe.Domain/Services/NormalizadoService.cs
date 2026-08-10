// Nome: [Pedro Henrique dos Santos]

using System.Text.RegularExpressions;

namespace AcademiaDoZe.Domain.Services;

public static partial class NormalizadoService
{
    public static bool TextoVazioOuNulo(string? texto) => string.IsNullOrWhiteSpace(texto);

    public static string LimparEspacos(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return string.Empty;
        return Regex.Replace(texto.Trim(), @"\s+", " ");
    }

    public static string LimparEDigitos(string? texto)
    {
        if (string.IsNullOrEmpty(texto)) return string.Empty;
        return Regex.Replace(texto, @"[^\d]", string.Empty);
    }

    public static string ParaMaiusculo(string? texto) => string.IsNullOrEmpty(texto) ? string.Empty : texto.ToUpperInvariant();

    public static string ParaMinusculo(string? texto) => string.IsNullOrEmpty(texto) ? string.Empty : texto.ToLowerInvariant();
}