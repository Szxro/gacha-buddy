using Account.Domain.Common;

namespace Account.Domain.Entities;

public class EmailCode : ExpirableTokenEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;
    
    public string Code { get; set; } = string.Empty;
}