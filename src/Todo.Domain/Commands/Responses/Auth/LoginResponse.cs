using System;

namespace Todo.Domain.Commands.Responses.Auth
{
  public class LoginResponse
  {
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
    public string RefreshToken { get; set; }
    public DateTime IssuedUtc { get; set; } = DateTime.UtcNow;
  }
}