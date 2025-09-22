using AttendanceSystem.Auth.API.Services;
using AttendanceSystem.Auth.ui;
using AttendanceSystem.Auth.ui.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration
var authApiBaseUrl = builder.Configuration["AuthApiBaseUrl"] ?? "https://localhost:7269/";
var importApiBaseUrl = builder.Configuration["ImportApiBaseUrl"] ?? "https://localhost:7002/";

// HTTP Client for Auth API (port 7269)
builder.Services.AddHttpClient("AuthAPI", client =>
{
    client.BaseAddress = new Uri(authApiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<AuthTokenHandler>();

// HTTP Client for Import API (port 7002) - for image upload and attendance operations
builder.Services.AddHttpClient("ImportAPI", client =>
{
    client.BaseAddress = new Uri(importApiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<AuthTokenHandler>();

// Add basic HttpClient for non-authenticated requests (default to Auth API)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(authApiBaseUrl)
});

// Authentication Services
builder.Services.AddBlazoredLocalStorage();  // Persistent storage
builder.Services.AddBlazoredSessionStorage(); // Session storage

// Authentication Providers
builder.Services.AddScoped<AuthTokenHandler>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

// Application Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ManagementService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<VacationRequestServices>();

// Update ImageServes to use the ImportAPI HttpClient
builder.Services.AddScoped<ImageServes>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("ImportAPI"); // Use ImportAPI client
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    return new ImageServes(httpClient, localStorage);
});

builder.Services.AddMudServices();

// Authorization Policies
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

await builder.Build().RunAsync();