using AttendanceSystem.ImportFile.ui;
using AttendanceSystem.ImportFile.ui.Pages;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5013/") });
builder.Services.AddMudServices();
builder.Services.AddScoped<AttendanceSystem.ImportFile.ui.Services.AttendanceService>();
builder.Services.AddTransient<IPlanAttendanceService, PlanAttendanceService>();
//builder.Services.AddScoped<AttendanceService>();

await builder.Build().RunAsync();
