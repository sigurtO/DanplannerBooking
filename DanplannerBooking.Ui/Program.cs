using Blazored.LocalStorage;
using DanplannerBooking.Ui;
using DanplannerBooking.Ui.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudBlazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ---------------------------------------------------
// 1) HTTP CLIENT TIL API
// ---------------------------------------------------
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7061/")
});

// ---------------------------------------------------
// 2) LOCAL STORAGE
// ---------------------------------------------------
builder.Services.AddBlazoredLocalStorage();

// ---------------------------------------------------
// 3) AUTHENTICATION (BLAZOR CLIENT-SIDE)
// ---------------------------------------------------
builder.Services.AddAuthorizationCore();

// REGISTRER VORES CUSTOM AUTH PROVIDER
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

//mudblazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
