namespace ramos_kyoto_hr.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando há tentativa de transição de estado inválida
/// </summary>
public class InvalidStateTransitionException : DomainException
{
    public string EntityName { get; }
    public object EntityId { get; }
    public string CurrentState { get; }
    public string AttemptedState { get; }

    public InvalidStateTransitionException(
        string entityName, 
        object entityId, 
        string currentState, 
        string attemptedState)
        : base($"{entityName} com ID '{entityId}' já está no estado '{currentState}'. Não é possível transicionar para '{attemptedState}'.")
    {
        EntityName = entityName;
        EntityId = entityId;
        CurrentState = currentState;
        AttemptedState = attemptedState;
    }

    public InvalidStateTransitionException(string message)
        : base(message)
    {
        EntityName = string.Empty;
        EntityId = string.Empty;
        CurrentState = string.Empty;
        AttemptedState = string.Empty;
    }
}

