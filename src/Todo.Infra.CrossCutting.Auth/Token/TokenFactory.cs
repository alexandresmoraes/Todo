using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Todo.Domain.Auth;
using Todo.Domain.Dto.Auth;
using Todo.Domain.Dto.Settings;
using Todo.Domain.Shared.Settings;
using Todo.Infra.CrossCutting.Auth.Entities;
using Todo.Infra.CrossCutting.Auth.Stores;

namespace Todo.Infra.CrossCutting.Auth.Token
{
  public class TokenFactory : ITokenFactory
  {
    private readonly UserManager<User> _userManager;
    private readonly UserStore<User, Role> _userStore;
    private readonly IOptions<AuthSettings> _authSettings;
    private readonly IOptions<JwtOptions> _jwtOptions;

    public TokenFactory(
      UserManager<User> userManager,
      UserStore<User, Role> userStore,
      IOptions<AuthSettings> authSettingsDto,
      IOptions<JwtOptions> jwtOptionsDto)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
      _authSettings = authSettingsDto ?? throw new ArgumentNullException(nameof(authSettingsDto));
      _jwtOptions = jwtOptionsDto ?? throw new ArgumentNullException(nameof(jwtOptionsDto));
    }

    public async Task<AccessTokenDto> GenerateToken(string username, string token = null, string refreshToken = null, CancellationToken cancellationToken = default)
    {
      if (token != null && refreshToken == null)
      {
        throw new ArgumentException("refreshToken null");
      }
      if (refreshToken != null && token == null)
      {
        throw new ArgumentException("token null");
      }

      var user = await GetUserByNameOrEmailAsync(username);
      var utcNow = DateTime.UtcNow;

      var claims = new Claim[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, user.NormalizedUserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.Value.IssuedAt).ToString(), ClaimValueTypes.Integer64),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.NormalizedUserName),
        new Claim(JwtRegisteredClaimNames.Email, user.NormalizedEmail),
        new Claim(TodoClaimTypes.UserId, user.Id.ToString()),
        new Claim(TodoClaimTypes.NameIdentifier, user.UserName)
      };

      var tokenHandler = new JwtSecurityTokenHandler();
      var secretKey = Encoding.ASCII.GetBytes(_authSettings.Value.SecretKey);

      var jwt = new JwtSecurityToken(
        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256),
        claims: claims,
        notBefore: utcNow,
        expires: utcNow.AddSeconds(_jwtOptions.Value.ExpiresIn),
        audience: _jwtOptions.Value.Audience,
        issuer: _jwtOptions.Value.Issuer
      );

      if (refreshToken != null)
      {
        var savedRefreshToken = await GetRefreshToken(username);

        if (savedRefreshToken != refreshToken)
          throw new SecurityTokenException("Invalid Refresh Token");

        await _userStore.RemoveTokenAsync(user, _jwtOptions.Value.Issuer, JwtBearerDefaults.AuthenticationScheme, cancellationToken);
      }

      refreshToken = GenerateRefreshToken();
      await _userStore.SetTokenAsync(user, _jwtOptions.Value.Issuer, JwtBearerDefaults.AuthenticationScheme, refreshToken, cancellationToken);

      return new AccessTokenDto(
        accessToken: tokenHandler.WriteToken(jwt),
        expiresIn: _jwtOptions.Value.ExpiresIn,
        tokenType: JwtBearerDefaults.AuthenticationScheme,
        refreshToken: refreshToken
      );
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var secretKey = Encoding.ASCII.GetBytes(_authSettings.Value.SecretKey);

      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = _jwtOptions.Value.Issuer,
        ValidateAudience = true,
        ValidAudience = _jwtOptions.Value.Audience,
        ValidateLifetime = false,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuerSigningKey = true
      };

      SecurityToken securityToken;
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
      var jwtSecurityToken = securityToken as JwtSecurityToken;
      if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        throw new SecurityTokenException("Invalid token");

      var expiresTick = principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
      var expiresTict = Convert.ToInt64(expiresTick);
      var expiresDate = DateTimeOffset.FromUnixTimeSeconds(expiresTict).DateTime;
      if (expiresDate.AddSeconds(_jwtOptions.Value.RefreshTokenExpires) < DateTime.UtcNow)
        throw new SecurityTokenException("Refresh token expires");

      return principal;
    }

    private string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using (var rng = RandomNumberGenerator.Create())
      {
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
      }
    }

    private async Task<string> GetRefreshToken(string username, CancellationToken cancellationToken = default)
    {
      var user = await GetUserByNameOrEmailAsync(username);
      if (user != null)
      {
        return await _userStore.GetTokenAsync(user, _jwtOptions.Value.Issuer, JwtBearerDefaults.AuthenticationScheme, cancellationToken);
      }

      throw new SecurityTokenException("Invalid username refreshtoken.");
    }

    private async Task<User> GetUserByNameOrEmailAsync(string username)
      => await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);

    private long ToUnixEpochDate(DateTime date)
      => (long)Math.Round((date.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds);
  }
}