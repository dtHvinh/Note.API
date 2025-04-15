using HtmlAgilityPack;

namespace BackEnd.WebCrawler;

public class Crawler
{
    public class LinkPreview
    {
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Image { get; set; } = default!;
    }

    // TODO: Sanitize or validate URLs before requesting.

    public static async Task<LinkPreview> Get(string url, CancellationToken cancellationToken = default)
    {
        using var httpClient = new HttpClient();
        var html = await httpClient.GetStringAsync(url, cancellationToken);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var title = htmlDoc.DocumentNode.SelectSingleNode("//title")?.InnerText?.Trim();

        var metaTags = htmlDoc.DocumentNode.SelectNodes("//meta");
        string? description = null;
        string? image = null;

        if (metaTags != null)
        {
            foreach (var tag in metaTags)
            {
                var nameAttr = tag.GetAttributeValue("name", "");
                var propAttr = tag.GetAttributeValue("property", "");
                var content = tag.GetAttributeValue("content", "");

                if (nameAttr == "description" || propAttr == "og:description")
                    description ??= content;

                if (propAttr == "og:image")
                    image ??= content;
            }
        }

        return new LinkPreview
        {
            Title = title ?? "No title found",
            Description = description ?? "No description found",
            Image = image ?? "No image found"
        };
    }
}
