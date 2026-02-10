public class ServerConfig
{
    public string Name { get; set; }
    public string BaseUrl { get; set; }
    public string Endpoint { get; set; } = "/smartdelivery-in/multi.php";
    public bool Enabled { get; set; } = true;
    public int Order { get; set; }
}