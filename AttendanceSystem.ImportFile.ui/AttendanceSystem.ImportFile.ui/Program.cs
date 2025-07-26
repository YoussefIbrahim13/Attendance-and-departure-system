using AttendanceSystem.ImportFile.ui;
using AttendanceSystem.ImportFile.ui.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5013/") });
        builder.Services.AddScoped<AttendanceSystem.ImportFile.ui.Services.AttendanceService>();
//builder.Services.AddScoped<AttendanceService>();

await builder.Build().RunAsync();
