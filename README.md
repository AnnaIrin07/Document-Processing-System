# Document Processing System

A backend system that allows users to upload PDF documents, queues them for background processing, extracts metadata, detects duplicates, and returns upload statuses.

---

## Features

- Upload PDF files (max 10MB)
- Metadata extraction (e.g., word count)
- Duplicate detection via SHA256 hash
- Background processing using in-memory queue
- Store documents and processing metadata in SQL Server
- Get status of uploaded files via API
- Unit testing using xUnit and Moq
- Clean architecture with Dependency Injection and SOLID principles

## Tech Stack

- ASP.NET Core 9.0
- Entity Framework Core 9 (Code First + SQL Server)
- HostedService (BackgroundService)
- Swagger UI
- xUnit + Moq for Unit Testing

---

## Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone https://github.com/AnnaIrin07/Document-Processing-System.git
   cd Document-Processing-System
