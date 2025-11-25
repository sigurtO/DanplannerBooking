using Blazored.LocalStorage;
using DanplannerBooking.Ui;
using DanplannerBooking.Ui.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudBlazor;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ---------------------------------------------------
// Local Storage
// ---------------------------------------------------
builder.Services.AddBlazoredLocalStorage();

// ---------------------------------------------------
// Authentication
// ---------------------------------------------------
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

//mudblazor
builder.Services.AddMudServices();
// ---------------------------------------------------
// Token Handler (tilf�jer JWT til alle requests)
// ---------------------------------------------------
builder.Services.AddTransient<TokenAuthorizationHandler>();

// ---------------------------------------------------
// HttpClient med handler
// ---------------------------------------------------
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7061/");
})
.AddHttpMessageHandler<TokenAuthorizationHandler>();

// ---------------------------------------------------
// Default HttpClient (bruges via DI)
// ---------------------------------------------------
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("ApiClient");
});

await builder.Build().RunAsync();
