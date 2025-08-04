using DocumentProcessing.Application.Interfaces;      
using DocumentProcessing.Infrastructure.Services;    
using DocumentProcessing.Infrastructure.Queue;
using DocumentProcessing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using DocumentProcessing.API.Configure;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// Configure Services (DI Container)
// ==============================

// 1. EF Core with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=ASDLAPKCH0478\\SQLEXPRESS;Database=DocumentDb;Trusted_Connection=True;TrustServerCertificate=True"));


// 2. Application & Infrastructure Services
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddSingleton<IBackgroundQueue, BackgroundDocumentQueue>();
builder.Services.AddHostedService<DocumentProcessingService>();

// 3. MVC & API Controllers
builder.Services.AddControllers();

// 4. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerFileOperationFilter>();
});


// 5. (Optional) CORS for frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// ==============================
// Configure Middleware Pipeline
// ==============================

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
