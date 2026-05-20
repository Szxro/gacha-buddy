using Account.Domain.Common;

namespace Account.Domain.Entities;

public class Permission : AuditableEntity
{
    public Permission()
    {
        RolePermissions = new List<RolePermission>();
    }
    
    public string Code { get; set; }  = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; }
}