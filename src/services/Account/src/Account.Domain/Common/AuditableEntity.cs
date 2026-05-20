namespace Account.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    
    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime ModifiedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
}