using DocumentProcessing.Application.DTOs;

namespace DocumentProcessing.Application.Interfaces
{
    public interface IBackgroundQueue
    {
        void Enqueue(DocumentQueueItem item);
        Task<DocumentQueueItem> DequeueAsync(CancellationToken cancellationToken);
    }
}
