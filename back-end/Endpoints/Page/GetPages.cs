using back_end.Dto;
using back_end.Goups;
using back_end.Utilities.Unknow;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace back_end.Endpoints.Page;

public class GetPageResponse
{
    public InfPageList<GetPageDto> Pages { get; set; } = default!;

}

public class GetPages : EndpointWithoutRequest<Results<Ok<GetPageResponse>, ErrorResponse>>
{
    public override void Configure()
    {
        Get("/");
        Group<PageGroup>();
    }

    public override async Task<Results<Ok<GetPageResponse>, ErrorResponse>> ExecuteAsync(CancellationToken ct)
    {
        var cursor = Query<Guid>("cursor", true);
        var limit = Query<int>("limit", true);

        await Task.FromResult(true);

        return new ErrorResponse() { Message = "Cursor: " + cursor + " Limit: " + limit + "" };
    }
}
