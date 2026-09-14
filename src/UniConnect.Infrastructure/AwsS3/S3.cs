// File: UniConnect.Infrastructure/AwsS3/R2StorageService.cs
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using UniConnect.Application.DTOs;
using UniConnect.Application.Interfaces;

namespace UniConnect.Infrastructure.AwsS3;

public class R2StorageService : IR2StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;

    public R2StorageService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _config = config;
    }

    public async Task<string> UploadCvAsync(Stream fileStream, string fileName, string contentType, Guid userId, CancellationToken token)
    {
        var bucketName = _config["R2:BucketName"];
        var publicDomain = _config["R2:PublicDomain"];
        var fileExtension = Path.GetExtension(fileName);

        // Name format: cvs/{userId}_{guid}.pdf
        var fileKey = $"cvs/{userId}_{Guid.NewGuid()}{fileExtension}";

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey,
            InputStream = fileStream,
            ContentType = contentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request, token);

        return $"{publicDomain}/{fileKey}";
    }

    public async Task<string> UploadMediaAsync(FileUploadDto fileDto, CancellationToken token)
    {
        var bucketName = _config["R2:BucketName"];
        var publicDomain = _config["R2:PublicDomain"];
        var fileExtension = Path.GetExtension(fileDto.FileName);

        // Name format: media/{guid}{extension}
        var fileKey = $"media/{Guid.NewGuid()}{fileExtension}";

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey,
            InputStream = fileDto.Content,
            ContentType = fileDto.ContentType,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request, token);

        return $"{publicDomain}/{fileKey}";
    }

    public async Task DeleteFileByKeyAsync(string fileUrl, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return;

        var bucketName = _config["R2:BucketName"];
        var publicDomain = _config["R2:PublicDomain"];

        // Extract file key from URL
        var fileKey = fileUrl.Replace($"{publicDomain}/", "");

        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, token);
    }
}