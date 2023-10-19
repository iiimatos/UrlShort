using Microsoft.EntityFrameworkCore;

namespace UrlShort.Services;

public class UrlShorteningService
{

  public const int NumberOfCharsInShortLink = 8;
  private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWYX1234567890@zabcdefghijklmnopqrstuvwyxz";
  private readonly Random _random = new();
  private readonly ApplicationDbContext _dbContext;

  public UrlShorteningService(ApplicationDbContext dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task<string> GenerateUniqueCode()
  {
    while (true)
    {
      const string chars = Alphabet;
      var code = new string(Enumerable.Repeat(chars, NumberOfCharsInShortLink)
        .Select(x => x[_random.Next(x.Length)]).ToArray());

      if (! await _dbContext.Urls.AnyAsync(s => s.Code == code))
      {
        return code;
      }
    }
  }
}