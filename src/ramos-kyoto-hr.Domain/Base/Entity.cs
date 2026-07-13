namespace ramos_kyoto_hr.Domain.Base;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }
    public DateOnly EffectiveStartDate { get; protected set; }
    public bool IsActive { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    
    protected Entity(DateOnly effectiveStartDate)
    {
        EffectiveStartDate = effectiveStartDate;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }
    
    protected Entity(Guid id, DateOnly effectiveStartDate, bool isActive, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        EffectiveStartDate = effectiveStartDate;
        IsActive = true;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    protected void Update(Action action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        OnBeforeUpdate();
        
        action();

        UpdatedAt = DateTime.UtcNow;
        
        OnAfterUpdate();
    }
    
    protected virtual void OnBeforeUpdate() { }
    protected virtual void OnAfterUpdate() { }
    
    public bool Equals(Entity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }
        
        return Id.Equals(other.Id);
    }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public override string ToString()
    {
        return $"{GetType().Name}: [{Id}]";
    }
}