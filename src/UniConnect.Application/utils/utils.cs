using System.Text.RegularExpressions;

public static class StorageValidationUtility
{
    private static readonly Regex CvKeyPattern = new Regex(
        @"(?:https?:\/\/[^\/]+\/)?cvs\/.+_[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\.[a-zA-Z0-9]+$",
        RegexOptions.Compiled
    );

    public static bool IsValidCvFileKey(string? fileKey)
    {
        if (string.IsNullOrWhiteSpace(fileKey))
        {
            return false;
        }

        return CvKeyPattern.IsMatch(fileKey);
    }
}