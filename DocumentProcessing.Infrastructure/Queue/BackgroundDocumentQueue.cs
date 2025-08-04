using DocumentProcessing.Application.DTOs;
using DocumentProcessing.Application.Interfaces;
using System.Threading.Channels;

namespace DocumentProcessing.Infrastructure.Queue
{
    public class BackgroundDocumentQueue : IBackgroundQueue
    {
        private readonly Channel<DocumentQueueItem> _queue;

        public BackgroundDocumentQueue()
        {
            _queue = Channel.CreateUnbounded<DocumentQueueItem>();
        }

        public void Enqueue(DocumentQueueItem item)
        {
            if (!_queue.Writer.TryWrite(item))
                throw new InvalidOperationException("Could not enqueue the document.");
        }

        public async Task<DocumentQueueItem> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
