using Microsoft.AspNetCore.Http;

namespace DocumentProcessing.Application.Interfaces;

public interface IDocumentService
{
    Task<Guid> UploadDocumentAsync(IFormFile file);
    Task<List<DocumentDto>> GetAllDocumentsAsync();
}
