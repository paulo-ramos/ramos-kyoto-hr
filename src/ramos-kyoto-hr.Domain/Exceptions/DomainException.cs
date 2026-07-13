namespace ramos_kyoto_hr.Domain.Exceptions;

/// <summary>
/// Exceção base para todos os erros de domínio
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

