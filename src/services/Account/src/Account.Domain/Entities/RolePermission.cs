namespace Account.Domain.Entities;

public class RolePermission
{
    public int PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;

    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}