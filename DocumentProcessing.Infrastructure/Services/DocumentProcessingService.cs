using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using DocumentProcessing.Application.Interfaces;
using DocumentProcessing.Infrastructure.Data;
using DocumentProcessing.Domain.Models;

namespace DocumentProcessing.Infrastructure.Services
{
    public class DocumentProcessingService : BackgroundService
    {
        private readonly IBackgroundQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public DocumentProcessingService(IBackgroundQueue queue, IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var item = await _queue.DequeueAsync(stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var document = await dbContext.Documents.FindAsync(item.DocumentId);
                if (document == null)
                    continue;

                try
                {
                    // Step 1: Set status to Processing
                    document.Status = DocumentStatus.Processing;
                    await dbContext.SaveChangesAsync(stoppingToken);

                    // Step 2: Simulate document processing 
                    await Task.Delay(1000, stoppingToken); 
                    var random = new Random();
                    document.WordCount = random.Next(100, 1000); 

                    // Step 3: Set status to Completed
                    document.Status = DocumentStatus.Completed;
                }
                catch
                {
                    document.Status = DocumentStatus.Failed;
                }

                await dbContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
