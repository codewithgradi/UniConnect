using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _claimsPrincipalFactory = claimsPrincipalFactory;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = dto.Email,
            Email = dto.Email,
            UserType = dto.UserType,
            IsActive = true,
            VerificationStatus = dto.UserType == UserType.Admin ? VerificationStatus.Approved : VerificationStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        string roleName = dto.UserType.ToString();
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }

        await _userManager.AddToRoleAsync(user, roleName);

        if (dto.UserType == UserType.Business)
        {
            var businessProfile = new BusinessProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CompanyName = dto.CompanyName ?? string.Empty
            };
            await _unitOfWork.BusinessProfiles.AddAsync(businessProfile, cancellationToken);
        }
        else
        {
            var userProfile = new UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                FirstName = dto.FirstName ?? string.Empty,
                LastName = dto.LastName ?? string.Empty,
                StudentNumber = dto.studentNumber ?? string.Empty,
                Programme = dto.Programme ?? string.Empty
            };
            await _unitOfWork.UserProfiles.AddAsync(userProfile, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Ok(new { message = "User registered successfully", userId = user.Id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Account is inactive." });
        }

        // 1. Generate ClaimsPrincipal (invokes your CustomClaimsPrincipalFactory automatically)
        var principal = await _claimsPrincipalFactory.CreateAsync(user);
        var claims = principal.Claims.ToList();

        // 2. Read JWT key from appsettings.json
        var jwtSecret = _configuration["Jwt:Secret"] ?? _configuration["JwtSettings:Secret"] ?? "YourSuperSecretKeyWithAtLeast32BytesLength!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. Build Token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // 4. Return DTO with explicit userType for easy frontend parsing
        return Ok(new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: Guid.NewGuid().ToString(),
            ExpiresIn: 28800,
            UserType: user.UserType.ToString(),
            VerificationStatus: user.VerificationStatus.ToString()
        ));
    }
}