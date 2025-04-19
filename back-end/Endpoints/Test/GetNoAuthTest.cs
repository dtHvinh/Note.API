using back_end.Utilities.Context;
using FastEndpoints;

namespace back_end.Endpoints.Test;

public class GetNoAuthTest : EndpointWithoutRequest<string>
{
    public AuthContext AuthContext { get; set; }

    public override void Configure()
    {
        Get("test/no-auth");
        AllowAnonymous();
    }

    public override async Task<string> ExecuteAsync(CancellationToken ct)
    {
        return "Hello from the back-end! " + AuthContext.UserId;
    }
}
