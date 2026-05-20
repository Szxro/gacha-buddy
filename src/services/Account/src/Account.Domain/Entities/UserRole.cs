namespace Account.Domain.Entities;

public class UserRole
{
    public int RoleId { get; set; }
    
    public Role Role { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}