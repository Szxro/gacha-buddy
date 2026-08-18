namespace Account.SharedKernel.Common.Messaging;

public class EmailMessage
{
    public string ToAddress { get; set; } = string.Empty;
    
    public string Subject { get; set; } = string.Empty;
    
    public string Body { get; set; } = string.Empty;
    
    public List<EmailAttachment>? Attachments { get; set; }
}

public class EmailAttachment
{
    public string FileName { get; set; } = string.Empty;

    public byte[] Content { get; set; } = [];

    public string ContentType { get; set; } = string.Empty;  // example: "application/pdf"
}