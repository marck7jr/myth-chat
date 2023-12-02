using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

using MudBlazor.Services;

using MythChat.ApiService.Kiota;
using MythChat.Web;
using MythChat.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddOutputCache();

builder.Services.AddScoped(provider =>
{
    var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();

    var httpClient = httpClientFactory.CreateClient();
    var authProvider = new AnonymousAuthenticationProvider();
    var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient)
    {
        BaseUrl = "http://apiservice/",
    };
    var client = new ApiServiceClient(adapter);

    return client;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
