// Nome: [Pedro Henrique dos Santos]

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;
using System.Collections.Generic;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Result<Cpf> Criar(string? valor)
    {
        var notifications = new List<Notification>();
        var valorNormalizado = NormalizadoService.LimparEDigitos(valor);

        if (string.IsNullOrWhiteSpace(valorNormalizado) || valorNormalizado.Length != 11 || !ValidarAlgoritmoCpf(valorNormalizado))
        {
            notifications.Add(new Notification("Cpf", "CPF em formato inválido."));
            return Result.Failure<Cpf>(notifications);
        }

        return Result.Success(new Cpf(valorNormalizado));
    }

    private static bool ValidarAlgoritmoCpf(string cpf)
    {
        if (cpf.Length != 11) return false;

        bool todosIguais = true;
        for (int i = 1; i < 11; i++)
        {
            if (cpf[i] != cpf[0]) { todosIguais = false; break; }
        }
        if (todosIguais) return false;

        int[] multiplicadores1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCpf = cpf.Substring(0, 9);
        int soma = 0;

        for (int i = 0; i < 9; i++)
            soma += (tempCpf[i] - '0') * multiplicadores1[i];

        int resto = soma % 11;
        int digito1 = resto < 2 ? 0 : 11 - resto;

        tempCpf += digito1;
        soma = 0;

        for (int i = 0; i < 10; i++)
            soma += (tempCpf[i] - '0') * multiplicadores2[i];

        resto = soma % 11;
        int digito2 = resto < 2 ? 0 : 11 - resto;

        return cpf.EndsWith(digito1.ToString() + digito2.ToString());
    }
}