using DocumentProcessing.Domain.Models;

public class DocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
    public int WordCount { get; set; }
    public DateTime UploadDate { get; set; }
}
