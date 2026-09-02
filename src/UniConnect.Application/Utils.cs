using System.Text.RegularExpressions;
namespace UniConnect.Application;
public static class StudentEmailValidator
{
    // Matches numbers followed by @my.richfield.ac.za
    private static readonly Regex StudentEmailRegex = new(
        @"^\d{7,10}@my\.richfield\.ac\.za$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool IsValidRichfieldStudent(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return StudentEmailRegex.IsMatch(email.Trim());
    }
}