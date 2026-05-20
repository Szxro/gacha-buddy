using Account.Domain.Common;

namespace Account.Domain.Entities;

public class Credentials : AuditableEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;

    public string HashValue { get; set; } = string.Empty;
    
    public string SaltValue { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime? LastUsedAt { get; set; }
}