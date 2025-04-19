using FastEndpoints;

namespace back_end.Goups;

public class PageGroup : Group
{
    public PageGroup()
    {
        Configure("pages", ep =>
        {
        });
    }
}
