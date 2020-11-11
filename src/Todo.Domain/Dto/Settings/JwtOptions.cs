using System;

namespace Todo.Domain.Dto.Settings
{
  public class JwtOptions
  {
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public string Audience { get; set; }
    public int ExpiresIn { get; set; }
    public int RefreshTokenExpires { get; set; }
    public DateTime Expiration => IssuedAt.Add(TimeSpan.FromMinutes(ExpiresIn));
    public DateTime NotBefore => DateTime.UtcNow;
    public DateTime IssuedAt => DateTime.UtcNow;
  }
}