using back_end.Model;
using back_end.Utilities.Provider;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace back_end.Endpoints.Auth;

public record RegisterRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public record RegisterResponse
{
    public bool IsSuccess { get; set; }
    public string[]? Errors { get; set; }
    public string? AccessToken { get; set; }
}

public class Register : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly UserManager<ApplicationUser> UserManager = default!;
    private readonly JwtTokenProvider JwtTokenProvider = default!;

    public override void Configure()
    {
        Post("/auth/register");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Register a new user";
            s.Description = "Register a new user with email and password";
            s.Response<RegisterResponse>(StatusCodes.Status200OK, "User registered successfully");
            s.Response<ErrorResponse>(StatusCodes.Status400BadRequest, "Invalid request");
            s.Response<ErrorResponse>(StatusCodes.Status500InternalServerError, "Internal server error");
        });
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var createResult = await UserManager.CreateAsync(CreateUser(req.Email), req.Password);

        if (!createResult.Succeeded)
        {
            await SendAsync(new()
            {
                AccessToken = string.Empty,
                Errors = createResult.Errors.Select(e => e.Description).ToArray(),
                IsSuccess = false
            }, StatusCodes.Status400BadRequest, ct);

            return;
        }
        var user = await UserManager.FindByEmailAsync(req.Email)!;

        var token = await JwtTokenProvider.CreateTokenAsync(user!);

        await SendOkAsync(new RegisterResponse { AccessToken = token }, ct);
    }

    static ApplicationUser CreateUser(string email)
    {
        return new ApplicationUser()
        {
            Email = email,
        };
    }
}
