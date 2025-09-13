using Applications.CSVFile.Querys.UploadCSVFilequery;
using Applications.Employees.Commands.UpdataEmployeecommand;
using Applications.Employees.profiles;
using Applications.Employees.Querys.GetEmployeeByCode;
using Applications.UpdateAttendanceRecord.Commands;
using AttendanceSystem.ImportFile.API.Services.AttendanceServices;
using EmployeesModels.Shared.Data;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using MediatR;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbcontext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);



// Replace the incorrect AutoMapper registration line with the correct usage.
// The method 'RegisterServicesFromAssemblies' does not exist on IMapperConfigurationExpression.
// To scan for profiles in the current assembly, use AddAutoMapper and pass the assembly.
builder.Services.AddAutoMapper(typeof(UploadCSVFilequery).Assembly);
builder.Services.AddAutoMapper(typeof(EmployeeProfile).Assembly);


//builder.Services.AddTransient<IAttendanceService, AttendanceService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(UploadCSVFilequery).Assembly);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod()
);
app.UseHttpsRedirection();

app.UseStaticFiles();
var profileImagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile_images");
if (!Directory.Exists(profileImagesPath))
{
    Directory.CreateDirectory(profileImagesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(profileImagesPath),
    RequestPath = "/profile_images"
});
app.UseAuthorization();

app.MapControllers();

app.Run();