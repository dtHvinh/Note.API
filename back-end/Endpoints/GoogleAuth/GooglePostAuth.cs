using back_end.Data;
using back_end.Services;
using back_end.Utilities.Context;
using back_end.Utilities.Provider;
using back_end.Utilities.Service;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

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
    public UserService UserService { get; set; } = default!;
    public ApplicationDbContext ApplicationDbContext { get; set; } = default!;
    public AuthContext AuthContext { get; set; } = default!;

    public override void Configure()
    {
        Post("auth/google");
        AllowAnonymous();
    }

    public override async Task<Results<Ok<AuthResponse>, ErrorResponse>> ExecuteAsync(AuthRequest req, CancellationToken ct)
    {
        var (isValidToken, payload) = TokenValidationService.ValidateGooleIdToken(req.AccessToken!);
        if (!isValidToken)
            return new ErrorResponse() { Message = "Invalid token" };

        var email = payload.GetValueOrDefault("email", "").ToString()!;
        var username = payload.GetValueOrDefault("given_name", "").ToString()!;
        var picture = payload.GetValueOrDefault("picture", "").ToString()!;

        if (email is null || username is null)
            return new ErrorResponse() { Message = "Invalid Token" };

        var (isSuccess, strValue) = await UserService.GoogleLoginAsync(email, username, picture);
        if (isSuccess)
        {
            return TypedResults.Ok(new AuthResponse
            {
                AccessToken = strValue
            });
        }
        else
        {
            return new ErrorResponse() { Message = strValue };
        }
    }
}
