public class ServerState
{
    public string Name { get; set; }
    public string BaseUrl { get; set; }
    public string Endpoint { get; set; }
    public bool Enabled { get; set; }
    public int Order { get; set; }

    public bool IsAvailable { get; set; } = true;
    public DateTime? LastFailureTime { get; set; }
    public int FailureCount { get; set; }
    public DateTime? LastSuccessTime { get; set; }

    public bool IsCooldown =>
        LastFailureTime.HasValue &&
        DateTime.UtcNow - LastFailureTime.Value < TimeSpan.FromMinutes(5);
}