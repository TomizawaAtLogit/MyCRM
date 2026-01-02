using System.Net;

namespace AspireApp1.Web;

public class CookieForwardingHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookieForwardingHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        
        // Configure to use Windows credentials
        if (InnerHandler is HttpClientHandler handler)
        {
            handler.UseDefaultCredentials = true;
            handler.Credentials = CredentialCache.DefaultNetworkCredentials;
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // Forward cookies from the incoming request
            var cookies = httpContext.Request.Headers.Cookie;
            if (!string.IsNullOrEmpty(cookies))
            {
                request.Headers.TryAddWithoutValidation("Cookie", cookies.ToString());
            }

            // Forward authentication header if present
            var authHeader = httpContext.Request.Headers.Authorization;
            if (!string.IsNullOrEmpty(authHeader))
            {
                request.Headers.TryAddWithoutValidation("Authorization", authHeader.ToString());
            }

            // Add the Windows username as a custom header
            var username = httpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(username))
            {
                request.Headers.TryAddWithoutValidation("X-Forwarded-User", username);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
