using BackEnd.WebCrawler;
using FastEndpoints;

namespace back_end.Endpoints.FetchUrl;

public record FetchUrlResponse
{
    public required bool Success { get; set; }
    public required string Link { get; set; }
    public required FetchMeta Meta { get; set; }

    public record FetchMeta
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public FetchMetaImage? Image { get; set; }

        public record FetchMetaImage
        {
            public string? Url { get; set; }
        }
    }
}

public class FetchUrlData : EndpointWithoutRequest<FetchUrlResponse>
{
    public override void Configure()
    {
        Get("/api/fetch-url");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var urlQuery = Query<string>("url", true);

        if (string.IsNullOrEmpty(urlQuery))
        {
            await SendNotFoundAsync(ct);

            return;
        }

        var linkPreview = await Crawler.Get(urlQuery, ct);

        var response = new FetchUrlResponse
        {
            Success = true,
            Link = urlQuery,
            Meta = new FetchUrlResponse.FetchMeta
            {
                Title = linkPreview.Title,
                Description = linkPreview.Description,
                Image = new FetchUrlResponse.FetchMeta.FetchMetaImage
                {
                    Url = linkPreview.Image
                }
            }
        };

        await SendAsync(response, cancellation: ct);
    }
}
