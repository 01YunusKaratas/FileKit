using System;
using Microsoft.AspNetCore.Http;
using FileKit.Entity.Dtos;

namespace FileKit.Interface;

public interface IFileService
{
    Task<string> UploadAsync(IFormFile file, string directory); // return file path
    Task<bool> DeleteAsync(string filePath);// return success or failure
    Task<bool> ExistsAsync(string filePath);
    Task<bool> MoveAsync(string sourcePath, string destinationPath);// return success or failure
    Task<byte[]> DownloadAsync(string filePath);// dosyayı indirmek için byte dizisi döner
    Task<FileInfo<T>> GetFileInfoAsync<T>(string filePath, T? extraData = default); // Ekstra veri ile birlikte bilgileri döner
}
