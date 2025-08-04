namespace DocumentProcessing.Application.DTOs
{
    public class DocumentQueueItem
    {
        public Guid DocumentId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileHash { get; set; } = string.Empty;
    }
}
