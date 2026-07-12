using System.Text.RegularExpressions;

namespace ramos_kyoto_hr.Domain.ObjectValue;

public sealed class Cnpj : IEquatable<Cnpj>
{
    public string Valor { get; }
    
    private Cnpj(string valor)
    {
        Valor = valor;
    }

    public static Cnpj Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("O CNPJ não pode ser vazio ou nulo.", nameof(valor));

        var valorLimpo = ClearCnpj(valor);

        if (valorLimpo.Length != 14)
            throw new ArgumentException("O CNPJ deve conter exatamente 14 caracteres alfanuméricos.", nameof(valor));

        if (!ValidateCnpj(valorLimpo))
            throw new ArgumentException("O CNPJ fornecido é inválido conforme as regras da Receita Federal.", nameof(valor));

        return new Cnpj(valorLimpo);
    }

    private static string ClearCnpj(string valor)
    {
        return Regex.Replace(valor, @"[^A-Za-z0-9]", "").ToUpper();
    }

    private static bool ValidateCnpj(string cnpj)
    {
        if (cnpj.Distinct().Count() == 1) return false;

        if (!char.IsDigit(cnpj[12]) || !char.IsDigit(cnpj[13])) return false;

        int[] pesosDV1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] pesosDV2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        var valoresConvertidos = new int[12];
        for (var i = 0; i < 12; i++)
        {
            valoresConvertidos[i] = cnpj[i] - 48;
        }

        var soma1 = 0;
        for (var i = 0; i < 12; i++)
        {
            soma1 += valoresConvertidos[i] * pesosDV1[i];
        }

        var resto1 = soma1 % 11;
        var dv1Esperado = resto1 < 2 ? 0 : 11 - resto1;

        var valoresComDV1 = new int[13];
        Array.Copy(valoresConvertidos, valoresComDV1, 12);
        valoresComDV1[12] = dv1Esperado;

        var soma2 = 0;
        for (var i = 0; i < 13; i++)
        {
            soma2 += valoresComDV1[i] * pesosDV2[i];
        }

        var resto2 = soma2 % 11;
        var dv2Esperado = resto2 < 2 ? 0 : 11 - resto2;

        var dvInformado1 = int.Parse(cnpj[12].ToString());
        var dvInformado2 = int.Parse(cnpj[13].ToString());

        return dv1Esperado == dvInformado1 && dv2Esperado == dvInformado2;
    }

    public bool Equals(Cnpj? other)
    {
        return other is not null && Valor == other.Valor;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Cnpj);
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public override string ToString()
    {
        return Valor;
    }
    
    public static bool operator ==(Cnpj? left, Cnpj? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(Cnpj? left, Cnpj? right)
    {
        return !(left == right);
    }
    
    public static implicit operator string(Cnpj cnpj) => cnpj.Valor;
}