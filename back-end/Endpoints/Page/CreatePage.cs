using back_end.Data;
using back_end.Model;
using back_end.Utilities.Context;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace back_end.Endpoints.Page;

public record CreatePageRequest(string Name, string IconType, string IconData);

public class CreatePage(ApplicationDbContext dbContext, AuthContext authContext)
    : Endpoint<CreatePageRequest, Results<Ok, ErrorResponse>>
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly AuthContext _authContext = authContext;

    public override void Configure()
    {
        Post("page");
    }

    public override async Task<Results<Ok, ErrorResponse>> ExecuteAsync(CreatePageRequest req, CancellationToken ct)
    {
        _dbContext.Pages.Add(new Model.Page()
        {
            Name = req.Name,
            AuthorId = _authContext.UserId,
            PageIcon = new()
            {
                Type = Enum.Parse<PageIconTypes>(req.IconType),
                Data = req.IconData
            }
        });

        var result = await _dbContext.SaveChangesAsync(ct);

        return result == 1 ? TypedResults.Ok() : new ErrorResponse() { Message = "Fail to create page" };
    }
}
