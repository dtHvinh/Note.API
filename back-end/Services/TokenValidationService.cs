using System.IdentityModel.Tokens.Jwt;

namespace back_end.Services;

public sealed class TokenValidationService
{
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="ArgumentNullException"/>
    public static (bool, JwtPayload) ValidateGooleIdToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
        }
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = handler.ReadJwtToken(token);
            return (jwtToken.ValidTo > DateTime.UtcNow, jwtToken.Payload);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Invalid token", ex);
        }
    }
}
