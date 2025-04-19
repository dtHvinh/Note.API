using FastEndpoints;

namespace back_end.Endpoints.Test;

public class GetNoAuthTest : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("test/no-auth");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync("Hello from the back-end!", cancellation: ct);
    }
}
