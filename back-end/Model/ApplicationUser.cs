using Microsoft.AspNetCore.Identity;

namespace back_end.Model;

public class ApplicationUser : IdentityUser<int>
{
    public string? ProfilePicture { get; set; }
}
