////using AttendanceSystem.Auth.ui;
////using Microsoft.AspNetCore.Components.Web;
////using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

////var builder = WebAssemblyHostBuilder.CreateDefault(args);
////builder.RootComponents.Add<App>("#app");
////builder.RootComponents.Add<HeadOutlet>("head::after");

////builder.Services.AddScoped(sp => new HttpClient
////{
////    BaseAddress = new Uri("https://localhost:7269/") // change to your backend API base address
////});
////await builder.Build().RunAsync();

////using AttendanceSystem.Auth.API.Services;
////using AttendanceSystem.Auth.ui;
////using AttendanceSystem.Auth;
////using Blazored.LocalStorage;
////using Microsoft.AspNetCore.Components.Authorization;
////using Microsoft.AspNetCore.Components.Web;
////using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

////var builder = WebAssemblyHostBuilder.CreateDefault(args);
////builder.RootComponents.Add<App>("#app");
////builder.RootComponents.Add<HeadOutlet>("head::after");

////// Configure HttpClient with CORS support
////builder.Services.AddHttpClient("AuthAPI", client =>
////{
////    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
////    client.DefaultRequestHeaders.Accept.Add(
////        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
////})
////.AddHttpMessageHandler<AuthTokenHandler>();

////// For non-authenticated requests
////builder.Services.AddScoped(sp => new HttpClient
////{
////    BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"])
////});

////// Authentication services
////builder.Services.AddBlazoredLocalStorage();
////builder.Services.AddScoped<JwtAuthenticationStateProvider>();
////builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
////    sp.GetRequiredService<JwtAuthenticationStateProvider>());
////builder.Services.AddScoped<AuthService>();
////builder.Services.AddScoped<AuthTokenHandler>();

////// Register other services
////builder.Services.AddScoped<ErrorHandler>();

////await builder.Build().RunAsync();

////// Auth Token Handler
////public class AuthTokenHandler : DelegatingHandler
////{
////    private readonly ILocalStorageService _localStorage;

////    public AuthTokenHandler(ILocalStorageService localStorage)
////    {
////        _localStorage = localStorage;
////    }

////    protected override async Task<HttpResponseMessage> SendAsync(
////        HttpRequestMessage request,
////        CancellationToken cancellationToken)
////    {
////        var token = await _localStorage.GetItemAsync<string>("authToken");

////        if (!string.IsNullOrEmpty(token))
////        {
////            request.Headers.Authorization =
////                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
////        }

////        return await base.SendAsync(request, cancellationToken);
////    }
////}


//using AttendanceSystem.Auth.API.Services;
//using AttendanceSystem.Auth.ui;
//using Blazored.LocalStorage;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Components.Web;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
//using System;

//var builder = WebAssemblyHostBuilder.CreateDefault(args);
//builder.RootComponents.Add<App>("#app");
//builder.RootComponents.Add<HeadOutlet>("head::after");

//// Get the base URL from configuration with fallback
//var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7269/";

//// Configure HttpClient with CORS support
//builder.Services.AddHttpClient("AuthAPI", client =>
//{
//    client.BaseAddress = new Uri(apiBaseUrl);
//    client.DefaultRequestHeaders.Accept.Add(
//        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//})
//.AddHttpMessageHandler<AuthTokenHandler>();

//// For non-authenticated requests
//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri(apiBaseUrl)
//});

//// Authentication services
//builder.Services.AddBlazoredLocalStorage();
//builder.Services.AddScoped<JwtAuthenticationStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
//    sp.GetRequiredService<JwtAuthenticationStateProvider>());
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<AuthTokenHandler>();

//await builder.Build().RunAsync();

//public class AuthTokenHandler : DelegatingHandler
//{
//    private readonly ILocalStorageService _localStorage;

//    public AuthTokenHandler(ILocalStorageService localStorage)
//    {
//        _localStorage = localStorage;
//    }

//    protected override async Task<HttpResponseMessage> SendAsync(
//        HttpRequestMessage request,
//        CancellationToken cancellationToken)
//    {
//        var token = await _localStorage.GetItemAsync<string>("authToken");

//        if (!string.IsNullOrEmpty(token))
//        {
//            request.Headers.Authorization =
//                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//        }

//        return await base.SendAsync(request, cancellationToken);
//    }
//}

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