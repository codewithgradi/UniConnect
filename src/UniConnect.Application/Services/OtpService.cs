using Microsoft.Extensions.Caching.Distributed;

namespace UniConnect.Application.Services;

public interface IOtpService
{
    Task SaveOtpAsync(string email, string otp, CancellationToken ct = default);
    Task<bool> ValidateOtpAsync(string email, string otp, CancellationToken ct = default);
}

public class OtpService : IOtpService
{
    private readonly IDistributedCache _cache;
    private static readonly TimeSpan OtpLifetime = TimeSpan.FromMinutes(10);

    public OtpService(IDistributedCache cache)
    {
        _cache = cache;
    }

    private static string GetCacheKey(string email) => $"otp:{email.Trim().ToLowerInvariant()}";

    public async Task SaveOtpAsync(string email, string otp, CancellationToken ct = default)
    {
        var key = GetCacheKey(email);
        var options = new DistributedCacheEntryOptions
        {
            // Tells Redis to automatically delete this key after 10 minutes
            AbsoluteExpirationRelativeToNow = OtpLifetime
        };

        await _cache.SetStringAsync(key, otp, options, ct);
    }

    public async Task<bool> ValidateOtpAsync(string email, string otp, CancellationToken ct = default)
    {
        var key = GetCacheKey(email);
        var storedOtp = await _cache.GetStringAsync(key, ct);

        // If key doesn't exist or expired in Redis
        if (string.IsNullOrEmpty(storedOtp))
        {
            return false;
        }

        // Compare values
        if (storedOtp == otp)
        {
            // Code matched successfully -> Delete immediately so it can't be reused
            await _cache.RemoveAsync(key, ct);
            return true;
        }

        return false;
    }
}