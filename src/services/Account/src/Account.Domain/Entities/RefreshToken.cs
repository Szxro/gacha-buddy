using Account.Domain.Common;

namespace Account.Domain.Entities;

public class RefreshToken : ExpirableTokenEntity
{
    public int UserId { get; set; }

    public User User { get; set; } = null!;
    
    public string Token { get; set; } = string.Empty;
}