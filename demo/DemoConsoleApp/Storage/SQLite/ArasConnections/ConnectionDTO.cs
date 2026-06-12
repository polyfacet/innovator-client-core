public class ConnectionDTO
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string DB { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int? SortOrder { get; internal set; }
}