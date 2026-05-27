using Account.Domain.Common;

namespace Account.Domain.Entities;

public class Role : AuditableEntity
{
    public Role()
    {
        UserRoles = new List<UserRole>();
        RolePermissions = new List<RolePermission>();
    }
    
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; }
}