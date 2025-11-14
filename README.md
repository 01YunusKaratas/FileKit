# 🧰 FileKit — Simple & Powerful File Management for .NET

**FileKit** is a lightweight, modern file management library for .NET.  
It simplifies file operations like **upload, delete, move, download, and metadata retrieval** —  
so you never have to write “FileHelper” boilerplate code again.

---

## 🚀 Features

- ⚡️ One-line integration (`builder.Services.AddFileKit();`)
- 🧱 `IFormFile` and stream-based upload support
- 🧩 Fully **asynchronous (async/await)** implementation
- 📂 Automatic folder creation
- 🧠 `FileInfo<T>` generic model for custom metadata
- 🧾 Built-in logging via `ILogger`
- 🧰 Easily extendable (Azure Blob, AWS S3, Docker-ready)

---

## 📦 Installation

Install from **NuGet**:

```bash
dotnet add package FileKit
```

## Using FileKit

=> Program.cs
```
using FileKit.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register FileKit
builder.Services.AddFileKit();

var app = builder.Build();
app.MapControllers();
app.Run();
```
=> Example Controller
```
using Microsoft.AspNetCore.Mvc;
using FileKit.Interface;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var path = await _fileService.UploadAsync(file, "uploads");
        return Ok(new { path });
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(string path)
    {
        var success = await _fileService.DeleteAsync(path);
        return Ok(new { success });
    }

    [HttpGet("download")]
    public async Task<IActionResult> Download(string path)
    {
        var bytes = await _fileService.DownloadAsync(path);
        return File(bytes, "application/octet-stream", Path.GetFileName(path));
    }

    [HttpGet("info")]
    public async Task<IActionResult> Info(string path)
    {
        var info = await _fileService.GetFileInfoAsync(path);
        return Ok(info);
    }
}
```
## FileInfo<T> — Extended File Metadata Model
FileKit provides detailed file metadata such as name, size, and creation date,
but you can also attach custom metadata dynamically using the generic FileInfo<T> model.
```
var info = await _fileService.GetFileInfoAsync("uploads/avatar.png", new {
    UploadedBy = "admin",
    Project = "FileKitDemo"
});
```
## JSON Response
```
{
  "fileName": "avatar.png",
  "filePath": "uploads/avatar.png",
  "fileSize": 1048576,
  "createdAt": "2025-11-12T14:22:00",
  "extraData": {
    "uploadedBy": "admin",
    "project": "FileKitDemo"
  }
}
```
