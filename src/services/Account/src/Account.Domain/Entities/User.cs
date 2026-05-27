using Account.Domain.Common;

namespace Account.Domain.Entities;

public class User : AuditableEntity
{
    public User()
    {
        Credentials = new List<Credentials>();
        AuditLogs = new List<AuditLog>();
        UserRoles = new List<UserRole>();
        RefreshTokens = new List<RefreshToken>();
        EmailCodes = new List<EmailCode>();
    }
    
    public string Firstname { get; set; } = string.Empty;
    
    public string Lastname { get; set; } = string.Empty;
    
    public string Username { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public bool IsEmailVerified { get; set; }

    public DateTime LockoutEnd { get; set; }

    public bool IsLockOutEnable { get; set; }

    public int AccessFailedCount { get; set; }

    public ICollection<Credentials> Credentials { get; set; }

    public ICollection<AuditLog> AuditLogs { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; }

    public ICollection<EmailCode> EmailCodes { get; set; }
}