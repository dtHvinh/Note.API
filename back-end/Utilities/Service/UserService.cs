using back_end.Attributes;
using back_end.Data;
using back_end.Model;
using back_end.Utilities.Extensions;
using back_end.Utilities.Provider;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace back_end.Utilities.Service;

[Dependency(Lifetime = ServiceLifetime.Scoped)]
public class UserService(JwtTokenProvider jwtTokenProvider, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext)
{
    private readonly JwtTokenProvider _tokenProvider = jwtTokenProvider;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    /// <summary>
    /// Create new user
    /// </summary>
    /// <returns>If success return token, else return error</returns>
    public async Task<(bool, string)> GoogleLoginAsync(string email, string username, string picture)
    {
        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser() { Email = email, UserName = username.RemoveDiacritics(), ProfilePicture = picture };

            var createUser = await CreateGoogleUser(user);
            if (!createUser.Succeeded)
                return (false, createUser.Errors.First().Description);

            var createClaims = await CreateNewUserClaim(user);
            if (!createClaims.Succeeded)
                return (false, createClaims.Errors.First().Description);
        }

        var token = await _tokenProvider.CreateTokenAsync(user);
        return (true, token);
    }


    private async Task<IdentityResult> CreateNewUserClaim(ApplicationUser user)
    {
        var createClaims = await _userManager.AddClaimsAsync(user, [
              new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        ]);

        return createClaims;
    }

    private async Task<IdentityResult> CreateGoogleUser(ApplicationUser user)
    {
        var identityResult = await _userManager.CreateAsync(user);

        return identityResult;
    }
}
