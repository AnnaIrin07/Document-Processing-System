using Moq;
using Microsoft.AspNetCore.Http;
using DocumentProcessing.Infrastructure.Services;
using DocumentProcessing.Application.Interfaces;
using DocumentProcessing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Internal;

public class DocumentServiceTests
{
    private readonly DocumentService _service;
    private readonly Mock<IBackgroundQueue> _mockQueue;
    private readonly AppDbContext _context;
    private readonly Mock<IHostEnvironment> _mockEnv;

    public DocumentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _mockQueue = new Mock<IBackgroundQueue>();
        _mockEnv = new Mock<IHostEnvironment>();
        _mockEnv.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

        _service = new DocumentService(_context, _mockQueue.Object, _mockEnv.Object);
    }

    [Fact]
public async Task UploadDocumentAsync_DuplicateFile_ReturnsSameDocumentId()
{
    // Arrange
    var content = "Duplicate content";
    var fileName = "duplicate.pdf";
    var ms1 = new MemoryStream(Encoding.UTF8.GetBytes(content));
    var formFile1 = new FormFile(ms1, 0, ms1.Length, "file", fileName)
    {
        Headers = new HeaderDictionary(),
        ContentType = "application/pdf"
    };

    var ms2 = new MemoryStream(Encoding.UTF8.GetBytes(content));
    var formFile2 = new FormFile(ms2, 0, ms2.Length, "file", fileName)
    {
        Headers = new HeaderDictionary(),
        ContentType = "application/pdf"
    };

    // Act
    var firstId = await _service.UploadDocumentAsync(formFile1);
    var secondId = await _service.UploadDocumentAsync(formFile2);

    // Assert
    Assert.Equal(firstId, secondId);
    Assert.Single(_context.Documents.ToList()); 
}

[Fact]
public async Task UploadDocumentAsync_EmptyFile_ThrowsException()
{
    // Arrange
    var ms = new MemoryStream(); 
    var formFile = new FormFile(ms, 0, 0, "file", "empty.pdf")
    {
        Headers = new HeaderDictionary(),
        ContentType = "application/pdf"
    };

    // Act & Assert
    var ex = await Assert.ThrowsAsync<ArgumentException>(() =>
        _service.UploadDocumentAsync(formFile));

    Assert.Equal("Invalid file", ex.Message);
}

}
