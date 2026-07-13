namespace ramos_kyoto_hr.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há tentativa de criar uma entidade que já existe
/// </summary>
public class DuplicateEntityException : DomainException
{
    public string EntityName { get; }
    public string DuplicateField { get; }
    public object DuplicateValue { get; }

    public DuplicateEntityException(string entityName, string duplicateField, object duplicateValue)
        : base($"Já existe um(a) {entityName} com {duplicateField} = '{duplicateValue}'.")
    {
        EntityName = entityName;
        DuplicateField = duplicateField;
        DuplicateValue = duplicateValue;
    }

    public DuplicateEntityException(string message)
        : base(message)
    {
        EntityName = string.Empty;
        DuplicateField = string.Empty;
        DuplicateValue = string.Empty;
    }
}

