namespace Infrastructure.Options;

public class EmailOptions
{
    public const string SectionName = "EmailSettings";

    public required string DefaultFromAddress { get; set; }
    public required string DefaultFromName { get; set; }
}