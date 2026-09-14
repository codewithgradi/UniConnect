// File: UniConnect.Application/Common/Interfaces/IR2StorageService.cs
using System.IO;

namespace UniConnect.Application.Interfaces;

public record FileUploadDto(Stream Content, string FileName, string ContentType);

public interface IR2StorageService
{
    Task<string> UploadCvAsync(Stream fileStream, string fileName, string contentType, Guid userId, CancellationToken token);
    Task<string> UploadMediaAsync(FileUploadDto fileDto, CancellationToken token);
    Task DeleteFileByKeyAsync(string fileKey, CancellationToken token);
}