using AspireApp1.FrontEnd;
using AspireApp1.FrontEnd.Components;
using AspireApp1.Web;
using AspireApp1.Web.Services;
using Microsoft.AspNetCore.Authentication.Negotiate;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add localization service as singleton so language persists across circuits
builder.Services.AddSingleton<LocalizationService>();

// Add Google Maps service
builder.Services.AddSingleton<GoogleMapsService>();

builder.Services.AddOutputCache();

// Add Windows Authentication - DISABLED FOR LOCAL DEVELOPMENT
// builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
//     .AddNegotiate();

// Add Authorization with AdminOnly policy - DISABLED FOR LOCAL DEVELOPMENT
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy =>
//     {
//         policy.RequireAuthenticatedUser();
//         // In the frontend, we use a simple role-based check
//         // The actual authorization is enforced by the backend API
//         policy.RequireAssertion(context => 
//         {
//             // Allow access if user is authenticated
//             // Backend will enforce actual admin permissions
//             return context.User.Identity?.IsAuthenticated == true;
//         });
//     });
// });

// builder.Services.AddCascadingAuthenticationState();

// Add HttpContextAccessor for cookie forwarding
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CookieForwardingHandler>();

builder.Services.AddHttpClient<ProjectsApiClient>(client =>
    {
        // Allow overriding the DB API base URL in configuration for local development.
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<CustomerApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<ProjectActivityApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<ProjectTaskApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<AdminApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<OrderApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<AuditApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<EntityFilesApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<UserPreferencesApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<CasesApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<CaseActivityApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<RequirementDefinitionsApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<PreSalesProposalsApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

builder.Services.AddHttpClient<PreSalesActivitiesApiClient>(client =>
    {
        var dbApiBase = builder.Configuration["DbApiBaseUrl"];
        if (!string.IsNullOrWhiteSpace(dbApiBase))
        {
            client.BaseAddress = new(dbApiBase);
        }
        else
        {
            client.BaseAddress = new("https+http://dbapi");
        }
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseDefaultCredentials = true,
        Credentials = System.Net.CredentialCache.DefaultNetworkCredentials
    })
    .AddHttpMessageHandler<CookieForwardingHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// DISABLED FOR LOCAL DEVELOPMENT
// app.UseAuthentication();
// app.UseAuthorization();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
