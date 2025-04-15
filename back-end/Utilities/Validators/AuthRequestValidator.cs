using back_end.Endpoints.GoogleAuth;
using FastEndpoints;
using FluentValidation;
using System.IdentityModel.Tokens.Jwt;

namespace back_end.Utilities.Validators;

public class AuthRequestValidator : Validator<AuthRequest>
{
    public AuthRequestValidator()
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        RuleFor(x => x.AccessToken)
            .Must(TokenValidation)
            .WithMessage("Access token is empty or need to be well-formed");
    }

    private bool TokenValidation(string? token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return !string.IsNullOrEmpty(token) && tokenHandler.CanReadToken(token);
    }
}
