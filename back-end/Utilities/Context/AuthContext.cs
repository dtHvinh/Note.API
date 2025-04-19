using back_end.Attributes;
using back_end.Model.Base;
using System.Security.Claims;

namespace back_end.Utilities.Context;

[Dependency(Lifetime = ServiceLifetime.Transient)]
public class AuthContext(IHttpContextAccessor hca)
{
    private readonly HttpContext? _httpContext = hca.HttpContext;
    public bool IsResourceOwnedByUser(ICreatedByUser<int> resource) => UserId == resource.AuthorId;
    public int UserId => int.Parse(_httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
}
