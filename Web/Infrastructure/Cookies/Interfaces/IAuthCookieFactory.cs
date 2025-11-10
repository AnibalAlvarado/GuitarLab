using Microsoft.Extensions.Options;
namespace Web.Infrastructure.Cookies.Interfaces
{
    public interface IAuthCookieFactory
    {
        CookieOptions AccessCookieOptions(DateTimeOffset expires);
        CookieOptions RefreshCookieOptions(DateTimeOffset expires);
        CookieOptions CsrfCookieOptions(DateTimeOffset expires);
    }
}
