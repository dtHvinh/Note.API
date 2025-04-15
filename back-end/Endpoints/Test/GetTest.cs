using FastEndpoints;

namespace back_end.Endpoints.Test;

public class GetTest : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/api/test");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await SendAsync("Hello from the back-end!", cancellation: ct);
    }
}
