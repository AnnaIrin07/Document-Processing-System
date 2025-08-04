using DocumentProcessing.Application.DTOs;
using DocumentProcessing.Application.Interfaces;
using DocumentProcessing.Domain.Models;
using DocumentProcessing.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace DocumentProcessing.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IBackgroundQueue _queue;
        private readonly IHostEnvironment _env;

        public DocumentService(AppDbContext dbContext, IBackgroundQueue queue, IHostEnvironment env)
        {
            _dbContext = dbContext;
            _queue = queue;
            _env = env;
        }

        public async Task<Guid> UploadDocumentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            if (!file.FileName.EndsWith(".pdf") || file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("Only PDF files under 10MB are allowed");

            string fileHash = await ComputeFileHashAsync(file);

            var existing = await _dbContext.Documents.FirstOrDefaultAsync(d => d.FileHash == fileHash);
            if (existing != null)
                return existing.Id;

            string uploadsPath = Path.Combine(_env.ContentRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            string fileName = Guid.NewGuid() + ".pdf";
            string fullPath = Path.Combine(uploadsPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var document = new Document
            {
                FileName = file.FileName,
                FilePath = fullPath,
                FileHash = fileHash,
                UploadDate = DateTime.UtcNow,
                Status = DocumentStatus.Pending
            };

            _dbContext.Documents.Add(document);
            await _dbContext.SaveChangesAsync();

            _queue.Enqueue(new DocumentQueueItem
            {
                DocumentId = document.Id,
                FilePath = fullPath,
                FileHash = fileHash
            });

            return document.Id;
        }

        public async Task<List<DocumentDto>> GetAllDocumentsAsync()
        {
            return await _dbContext.Documents
                .OrderByDescending(d => d.UploadDate)
                .Select(d => new DocumentDto
                {
                    Id = d.Id,
                    FileName = d.FileName,
                    Status = d.Status,
                    UploadDate = d.UploadDate,
                    WordCount = d.WordCount
                })
                .ToListAsync();
        }

        private async Task<string> ComputeFileHashAsync(IFormFile file)
        {
            using var sha256 = SHA256.Create();
            using var stream = file.OpenReadStream();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
