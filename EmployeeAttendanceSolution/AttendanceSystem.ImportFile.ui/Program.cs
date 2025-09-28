using Microsoft.AspNetCore.Components.Authorization;

using AttendanceSystem.ImportFile.ui;
using AttendanceSystem.ImportFile.ui.Pages.nvvm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Blazored.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5013/") });
builder.Services.AddMudServices();
builder.Services.AddScoped<AttendanceSystem.ImportFile.ui.Services.AttendanceService>();
builder.Services.AddTransient<IPlanAttendanceService, PlanAttendanceService>();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<AttendanceSystem.ImportFile.ui.Services.BaseHTTPService>((sp) =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var localStorage = sp.GetRequiredService<ILocalStorageService>();
    return new AttendanceSystem.ImportFile.ui.Services.BaseHTTPService(httpClient, localStorage);
});
//builder.Services.AddScoped<AttendanceService>();


// إضافة خدمات المصادقة والأدوار
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, AttendanceSystem.ImportFile.ui.Services.JwtAuthenticationStateProvider>();
await builder.Build().RunAsync();
