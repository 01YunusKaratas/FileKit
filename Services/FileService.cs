using System;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using FileKit.Interface;
using Microsoft.AspNetCore.Http;
using FileKit.Entity.Dtos;


namespace FileKit.Services;

public class FileService:IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IWebHostEnvironment _env;
    
    public FileService(ILogger<FileService> logger, IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;   
    }

   public async Task<string> UploadAsync(IFormFile file, string directory)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");

            try
            {
                // Upload klasörünün tam yolunu oluştur
                var folderPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, directory);

                // Klasör yoksa oluştur
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                // Benzersiz dosya adı oluştur
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(folderPath, fileName);

                // Dosyayı diske kaydet
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Göreli path döndür (örneğin: "uploads/images/xyz.png")
                var relativePath = Path.Combine(directory, fileName).Replace("\\", "/");
                _logger.LogInformation("File uploaded successfully: {path}", relativePath);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while uploading file");
                throw;
            }
        }

        // Dosya silme
        public async Task<bool> DeleteAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, filePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted: {path}", filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while deleting file: {path}", filePath);
                return false;
            }
        }

        // Dosya var mı kontrolü
        public async Task<bool> ExistsAsync(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, filePath);
            var exists = File.Exists(fullPath);
        return exists; 
        }

        // Dosya taşıma
        public async Task<bool> MoveAsync(string sourcePath, string destinationPath)
        {
            try
            {
                var root = _env.WebRootPath ?? _env.ContentRootPath;
                var fullSource = Path.Combine(root, sourcePath);
                var fullDestination = Path.Combine(root, destinationPath);

                var destDir = Path.GetDirectoryName(fullDestination);
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir!);

                File.Move(fullSource, fullDestination, overwrite: true);
                _logger.LogInformation("File moved from {src} to {dest}", sourcePath, destinationPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving file");
                return false;
            }
        }

        // Dosya indirme (byte dizisi olarak döner)
        public async Task<byte[]> DownloadAsync(string filePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, filePath);
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", fullPath);

            var bytes = await File.ReadAllBytesAsync(fullPath);
            _logger.LogInformation("File downloaded: {path}", filePath);
            return bytes;
        }

        // Dosya bilgisi (Ekstra veri ile birlikte)
        public async Task<FileInfo<T>> GetFileInfoAsync<T>(string filePath, T? extraData = default)
        {
            var fullPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, filePath);
            var file = new System.IO.FileInfo(fullPath);

            if (!file.Exists)
                throw new FileNotFoundException("File not found", fullPath);

            var info = new FileInfo<T>
            {
                FileName = file.Name,
                FilePath = filePath.Replace("\\", "/"),
                FileSize = file.Length,
                CreatedAt = file.CreationTime,
                ExtraData = extraData
            };

            _logger.LogInformation("File info with extra data retrieved: {file}", filePath);
            return info;
        }
   
}   
