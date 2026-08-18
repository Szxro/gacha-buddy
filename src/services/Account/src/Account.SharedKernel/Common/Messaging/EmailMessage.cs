namespace Account.SharedKernel.Common.Messaging;

public class EmailMessage
{
    public string Subject { get; set; } = string.Empty;
    
    public string FromAddress { get; set; } = string.Empty;
    
    public string ToAddress { get; set; } = string.Empty;
    
    public string Body { get; set; } = string.Empty;
}