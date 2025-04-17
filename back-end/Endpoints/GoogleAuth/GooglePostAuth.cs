using back_end.Model;
using back_end.Services;
using back_end.Utilities.Extensions;
using back_end.Utilities.Provider;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace back_end.Endpoints.GoogleAuth;

public class AuthRequest
{
    public string? AccessToken { get; set; }
}

public class AuthResponse
{
    public required string AccessToken { get; set; }
}

public class GooglePostAuth :
    Endpoint<AuthRequest, Results<Ok<AuthResponse>, ErrorResponse>>
{
    public IHttpClientFactory ClientFactory { get; set; } = default!;
    public TokenValidationService TokenValidationService { get; set; } = default!;
    public JwtTokenProvider JwtTokenProvider { get; set; } = default!;
    public UserManager<ApplicationUser> UserManager { get; set; } = default!;

    public override void Configure()
    {
        Post("/api/auth/google");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<AuthResponse>, ErrorResponse>> ExecuteAsync(AuthRequest req, CancellationToken ct)
    {
        var (isValidToken, payload) = TokenValidationService.ValidateGooleIdToken(req.AccessToken!);
        if (!isValidToken)
            return new ErrorResponse() { Message = "Invalid token" };

        var email = payload.GetValueOrDefault("email", "").ToString();
        var username = payload.GetValueOrDefault("given_name", "").ToString();
        var picture = payload.GetValueOrDefault("picture", "").ToString();

        if (email is null || username is null)
            return new ErrorResponse() { Message = "Invalid Token" };

        var user = await UserManager.FindByEmailAsync(email!);
        if (user == null)
        {
            user = new() { Email = email, UserName = username.RemoveDiacritics(), ProfilePicture = picture };

            var createUser = await CreateGoogleUser(user);
            if (!createUser.Succeeded)
                return new ErrorResponse() { Message = createUser.Errors.First().Description };

            var createClaims = await CreateNewUserClaim(user);
            if (!createClaims.Succeeded)
                return new ErrorResponse() { Message = createClaims.Errors.First().Description };
        }

        var token = await JwtTokenProvider.CreateTokenAsync(user);

        return TypedResults.Ok(new AuthResponse
        {
            AccessToken = token
        });
    }

    private async Task<IdentityResult> CreateNewUserClaim(ApplicationUser user)
    {
        var createClaims = await UserManager.AddClaimsAsync(user, [
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        ]);

        return createClaims;
    }

    private async Task<IdentityResult> CreateGoogleUser(ApplicationUser user)
    {
        var password = RandomPasswordProvider.GeneratePassword(15)!;

        var identityResult = await UserManager.CreateAsync(user, password);

        return identityResult;
    }
}
