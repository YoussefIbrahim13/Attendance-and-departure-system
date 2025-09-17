
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
var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7269/";

// HTTP Client Configuration
builder.Services.AddHttpClient("AuthAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
})
.AddHttpMessageHandler<AuthTokenHandler>();

// Add basic HttpClient for non-authenticated requests
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
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
//builder.Services.AddScoped<SessionValidationService>();
builder.Services.AddScoped<ManagementService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddScoped<VacationRequestServices>();

/////
builder.Services.AddMudServices();

// Authorization Policies
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("AuthenticatedUser", policy => policy.RequireAuthenticatedUser());
});

await builder.Build().RunAsync();