using Account.Domain.Common;

namespace Account.Domain.Entities;

public class AuditLog : Entity
{
    public int? UserId { get; set; }

    public User? User { get; set; }

    public string Action { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public string UserAgent { get; set; } = string.Empty; // refer to the device that the user used

    public DateTime CreatedAt { get; set; }
}