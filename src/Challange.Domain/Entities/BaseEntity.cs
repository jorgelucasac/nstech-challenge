namespace Challange.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset DeletedAt { get; private set; }
    public bool IsActive { get; private set; }

    protected BaseEntity()
    {
        Id = Guid.CreateVersion7();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        DeletedAt = DateTimeOffset.UtcNow;
        IsActive = true;
    }

    public void UpdateTimestamps()
    {
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void SoftDelete()
    {
        IsActive = false;
        DeletedAt = DateTimeOffset.UtcNow;
    }
}