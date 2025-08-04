namespace DocumentProcessing.Domain.Models;

public enum DocumentStatus
{
    Pending,
    Processing,
    Completed,
    Duplicate,
    Failed
}

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
    public int WordCount { get; set; } = 0;
}
