namespace Settings;

public class SmstrafficSettings
{
    public const string SectionName = "Smstraffic";

    // Массив серверов
    public List<ServerConfig> Servers { get; set; } = new();

    // Учетные данные
    public string Login { get; set; }
    public string Password { get; set; }

    // Настройки
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public bool EnableSslValidation { get; set; } = true;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Login))
            throw new InvalidOperationException("Smstraffic Login is not configured");

        if (string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("Smstraffic Password is not configured");

        if (Servers == null || Servers.Count == 0)
            throw new InvalidOperationException("At least one SMS server must be configured");

        foreach (var server in Servers)
        {
            if (string.IsNullOrWhiteSpace(server.Name))
                throw new InvalidOperationException("Server name is required");

            if (string.IsNullOrWhiteSpace(server.BaseUrl))
                throw new InvalidOperationException($"BaseUrl is required for server '{server.Name}'");
        }
    }
}