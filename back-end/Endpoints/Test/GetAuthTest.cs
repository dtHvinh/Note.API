using back_end.Utilities.Context;
using FastEndpoints;

namespace back_end.Endpoints.Test;

public class GetAuthTest : EndpointWithoutRequest<string>
{
    public AuthContext Context { get; set; } = default!;

    public override void Configure()
    {
        Get("test/auth");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync("Hello user " + Context.UserId, cancellation: ct);
    }
}
