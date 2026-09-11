using Microsoft.AspNetCore.Mvc;
using UniConnect.Api.Controllers;
using UniConnect.Application.Services;
using UniConnect.Infrastructure.AwsS3;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ApiControllerBase
{
    private readonly IR2StorageService _r2Storage;
    private readonly IProfileService _profileService;

    public MediaController(IR2StorageService r2Storage, IProfileService profileService)
    {
        _r2Storage = r2Storage;
        _profileService = profileService;
    }
    [HttpPost("upload-cv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadCv(IFormFile file, CancellationToken token)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.ContentType != "application/pdf")
            return BadRequest("Only PDF documents are allowed.");

        var newCvUrl = await _r2Storage.UploadCvAsync(file, CurrentUserId, token);
        await _profileService.SaveCvUrlToDbAsync(CurrentUserId, newCvUrl, token);

        return Ok(new { Message = "CV uploaded successfully.", Url = newCvUrl });
    }
    [HttpPut("update-cv")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateCv([FromForm] CvUploadDto dto, CancellationToken token)
    {
        var file = dto.File;

        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        if (file.ContentType != "application/pdf")
            return BadRequest("Only PDF documents are allowed.");

        var newCvUrl = await _r2Storage.UploadCvAsync(file, CurrentUserId, token);
        var oldCvUrl = await _profileService.UpdateCvUrlInDbAsync(CurrentUserId, newCvUrl, token);

        if (!string.IsNullOrEmpty(oldCvUrl))
        {
            await _r2Storage.DeleteFileByKeyAsync(oldCvUrl, token);
        }

        return Ok(new { Message = "CV updated successfully.", Url = newCvUrl });
    }
}
public class CvUploadDto
{
    public required IFormFile File { get; set; }
}