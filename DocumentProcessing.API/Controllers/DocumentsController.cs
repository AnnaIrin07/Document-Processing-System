using DocumentProcessing.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocumentProcessing.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] DocumentUploadDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
            return BadRequest("No file uploaded.");

        try
        {
        var id = await _documentService.UploadDocumentAsync(dto.File);
        return Ok(new { DocumentId = id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }


        [HttpGet("status")]
        public async Task<IActionResult> GetAll()
        {
            var documents = await _documentService.GetAllDocumentsAsync();
            return Ok(documents);
        }
    }
}

