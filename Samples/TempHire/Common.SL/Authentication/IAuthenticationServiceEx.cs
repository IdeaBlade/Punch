using Cocktail;
using Security;

namespace Common.Authentication
{
    public interface IAuthenticationServiceEx : IAuthenticationService
    {
        UserPrincipal CurrentUser { get; }
    }
}