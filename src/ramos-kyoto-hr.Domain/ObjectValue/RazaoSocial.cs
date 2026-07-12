namespace ramos_kyoto_hr.Domain.ObjectValue;

public sealed class RazaoSocial : IEquatable<RazaoSocial>
{
    private const int TamanhoMinimo = 3;
    private const int TamanhoMaximo = 200;
    
    public string Valor { get; }

    private RazaoSocial(string valor)
    {
        Valor = valor;
    }

    public static RazaoSocial Create(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("A Razão Social não pode ser nula ou vazia.", nameof(valor));

        var valorTratado = valor.Trim();
        
        if (valorTratado.Length < TamanhoMinimo)
            throw new ArgumentException($"A Razão Social deve ter pelo menos {TamanhoMinimo} caracteres.", nameof(valor));

        if (valorTratado.Length > TamanhoMaximo)
            throw new ArgumentException($"A Razão Social não pode exceder {TamanhoMaximo} caracteres.", nameof(valor));

        return new RazaoSocial(valorTratado);
    }

    public bool Equals(RazaoSocial? other)
    {
        return other is not null && Valor == other.Valor;
    }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as RazaoSocial);
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public override string ToString()
    {
        return Valor;
    }

    public static bool operator ==(RazaoSocial? left, RazaoSocial? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(RazaoSocial? left, RazaoSocial? right)
    {
        return !(left == right);
    }

    public static implicit operator string(RazaoSocial razaoSocial) => razaoSocial.Valor;
}