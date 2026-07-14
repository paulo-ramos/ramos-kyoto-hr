namespace ramos_kyoto_hr.Domain.Base;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }
    public DateOnly EffectiveStartDate { get; protected set; }
    public bool IsActive { get; protected set; }
    public DateTime CreatedAt { get; private set; }
   
    
    protected Entity(DateOnly effectiveStartDate, bool isActive = true)
    {
        EffectiveStartDate = effectiveStartDate;
        IsActive = isActive;
        CreatedAt = DateTime.UtcNow;
    }
    
    protected Entity(Guid id, DateOnly effectiveStartDate, bool isActive, DateTime createdAt)
    {
        Id = id;
        EffectiveStartDate = effectiveStartDate;
        IsActive = true;
        CreatedAt = createdAt;
    }
    
    protected void ValidateNewEffectiveDate(DateOnly newEffectiveStartDate)
    {
        if (newEffectiveStartDate <= EffectiveStartDate)
        {
            throw new InvalidOperationException(
                $"A nova data efetiva ({newEffectiveStartDate:yyyy-MM-dd}) deve ser maior que a data efetiva atual ({EffectiveStartDate:yyyy-MM-dd})."
            );
        }
    }

    public bool IsStatusActive()
    {
        return this.IsActive;
    }
    
    public bool IsStatusDeactive()
    {
        return !this.IsActive;
    }
    
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