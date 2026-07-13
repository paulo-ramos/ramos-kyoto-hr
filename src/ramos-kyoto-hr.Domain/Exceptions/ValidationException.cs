namespace ramos_kyoto_hr.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há falha na validação de dados de entrada
/// </summary>
public class ValidationException : DomainException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message)
        : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }

    public ValidationException(string field, string errorMessage)
        : base($"Validação falhou para o campo '{field}': {errorMessage}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { errorMessage } }
        };
    }
}

