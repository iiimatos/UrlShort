namespace UrlShort.Models;

public class UrlManagement
{
  public int Id { get; set; }
  public string Url { get; set; } = string.Empty;
  public string ShortUrl { get; set; } = string.Empty;
  public string Code { get; set; } = string.Empty;
  public DateTime CreatedOnUtc { get; set; }
}