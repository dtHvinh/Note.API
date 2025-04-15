using back_end.Model;
using back_end.Utilities.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace back_end.Utilities.Provider;

public class JwtTokenProvider(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtOptions> optionAccessor)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly JwtOptions _options = optionAccessor.Value;
    private readonly SymmetricSecurityKey _symmetricSecurityKey = new(Encoding.UTF8.GetBytes(optionAccessor.Value.SecretKey));

    private async Task<string> CreateTokenInternalAsync(ApplicationUser user, IEnumerable<Claim>? externalClaims = null)
    {
        ArgumentNullException.ThrowIfNull(_options.SecretKey);
        SigningCredentials signingCredentials = new(_symmetricSecurityKey, _options.SecurityAlgorithm);
        JwtHeader header = new(signingCredentials);
        List<Claim> userClaims = [.. await _userManager.GetClaimsAsync(user)];
        if (externalClaims != null)
            userClaims.AddRange(externalClaims);
        JwtPayload payload = new(
            issuer: _options.Iss ?? throw new InvalidOperationException("\'Iss\' is null"),
            audience: _options.Aud ?? throw new InvalidOperationException("\'Aud\' is null"),
            claims: userClaims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_options.AccessTokenExpiredAfterMin));
        JwtSecurityToken securityToken = new(header, payload);
        JwtSecurityTokenHandler handler = new();
        return handler.WriteToken(securityToken);
    }

    public int GetIdFromToken(string accessToken)
    {
        JwtSecurityTokenHandler handler = new();

        var securityJwt = handler.ReadJwtToken(accessToken);

        var userId = int.Parse(securityJwt.Claims.First(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value);

        return userId;
    }

    public bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
        }

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var jwtToken = handler.ReadJwtToken(token);

            if (jwtToken.ValidTo == DateTime.MinValue)
            {
                return false;
            }

            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch (Exception ex) when (ex is ArgumentException || ex is SecurityTokenMalformedException)
        {
            return true;
        }
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<string> CreateTokenAsync(ApplicationUser user, IEnumerable<Claim>? externalClaims = null)
        => await CreateTokenInternalAsync(user, externalClaims);

    public string CreateToken(IEnumerable<Claim>? claims = null)
    {
        ArgumentNullException.ThrowIfNull(_options.SecretKey);

        SigningCredentials signingCredentials = new(_symmetricSecurityKey, _options.SecurityAlgorithm);

        JwtHeader header = new(signingCredentials);

        JwtPayload payload = new(
            issuer: _options.Iss ?? throw new InvalidOperationException("\'Iss\' is null"),
            audience: _options.Aud ?? throw new InvalidOperationException("\'Aud\' is null"),
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_options.AccessTokenExpiredAfterMin));

        JwtSecurityToken securityToken = new(header, payload);

        JwtSecurityTokenHandler handler = new();

        return handler.WriteToken(securityToken);
    }
}
