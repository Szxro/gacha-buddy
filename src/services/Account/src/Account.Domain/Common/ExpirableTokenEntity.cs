namespace Account.Domain.Common;

public abstract class ExpirableTokenEntity 
{
    public int Id { get; set; }
    
    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }
    
    public DateTime RevokedAt { get; set; }

    public DateTime ExpiredAt { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime ModifiedAt { get; set; }
}