using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using Account.Domain.Common;

namespace Account.Domain.Entities;

public class AuditLog : Entity
{
    public int? UserId { get; set; }

    public User? User { get; set; }

    public string RequestName { get; set; } = null!; // CommandName, QueryName

    public string? RequestData { get; set; } // Payload

    public string? ResourceName { get; set; } // refer to the entity (User)
    
    public string? ResourceId { get; set; }  // id of the entity

    public string IpAddress { get; init; } = string.Empty;

    public string UserAgent { get; init; } = string.Empty; // refer to the device that the user used
    
    public bool IsSuccessful { get; set; }

    public string? ErrorMessage { get; set; }

    public long? ExecutionTimeInMs { get; set; }
    
    public DateTime CreatedAt { get; init; }
}