namespace AcademiaDoZe.Infrastructure.Repositories;

/// <summary>
/// Regras de cálculo compartilhadas pelos repositórios de acesso.
/// Cada registro de acesso é apenas um carimbo de data/hora: o 1º do dia é entrada, o 2º é saída,
/// o 3º é entrada, e assim por diante (pares entrada/saída).
/// </summary>
internal static class AcessoCalculos
{
    public const string FormatoDataHora = "yyyy-MM-dd HH:mm:ss";

    public static string Formatar(DateTime dataHora) => dataHora.ToString(FormatoDataHora, System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Soma apenas os pares fechados (entrada + saída) de uma lista ordenada de horários.</summary>
    public static TimeSpan SomarPares(IReadOnlyList<DateTime> horariosOrdenados)
    {
        var total = TimeSpan.Zero;
        for (int i = 0; i + 1 < horariosOrdenados.Count; i += 2)
            total += horariosOrdenados[i + 1] - horariosOrdenados[i];
        return total;
    }
}
