public class AuthSettings
{
    public const string SectionName = "Authentication";

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
}