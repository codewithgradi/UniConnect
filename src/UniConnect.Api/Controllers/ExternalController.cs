using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.Services;
namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Student, Admin, Business")]
public class ExternalController : ControllerBase
{
    private IProfileService _profileService;

    public ExternalController(IProfileService profileService)
    {
        _profileService = profileService;
    }
    [HttpPost("save-to-blob")]
    public async Task<IActionResult> SaveToBlobDb()
    {
        string url = "";
        return Ok(new ResponseDtoFromBlob(url));
    }
}
public record ResponseDtoFromBlob(string url);