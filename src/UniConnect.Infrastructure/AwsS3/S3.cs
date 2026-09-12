using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace UniConnect.Infrastructure.AwsS3;

public interface IR2StorageService
{
    Task<string> UploadCvAsync(IFormFile file, Guid userId, CancellationToken token);
    Task DeleteFileByKeyAsync(string fileKey, CancellationToken token);
}

public class R2StorageService : IR2StorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _config;

    public R2StorageService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _config = config;
    }

    public async Task<string> UploadCvAsync(IFormFile file, Guid userId, CancellationToken token)
    {
        var bucketName = _config["R2:BucketName"];
        var publicDomain = _config["R2:PublicDomain"];
        var fileExtension = Path.GetExtension(file.FileName);

        // Name format: cvs/{userId}_{guid}.pdf
        var fileKey = $"cvs/{userId}_{Guid.NewGuid()}{fileExtension}";

        using var stream = file.OpenReadStream();
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey,
            InputStream = stream,
            ContentType = file.ContentType,
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

        // Extract "cvs/userId_guid.pdf" from "https://pub-xxx.r2.dev/cvs/userId_guid.pdf"
        var fileKey = fileUrl.Replace($"{publicDomain}/", "");

        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, token);
    }
}