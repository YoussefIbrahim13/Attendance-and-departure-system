using Applications.UpdateAttendanceRecord.Commands;
using AttendanceSystem.Auth.API.Services.Services.AuthoServices;
using AttendanceSystem.Auth.Services.Features.Users.Commands.AddUser;
using AttendanceSystem.Auth.Services.Features.Users.Commands.SendRandomPassword;
using AttendanceSystem.Auth.Services.Features.Users.Commands.UpdateUser;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.CreateVacationRequest;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetVacationRequestsByUserId;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.DBContext;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

//// Add services to the container.
//builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

// Identity configuration for lockout account after multiple failed attempts
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Lockout.AllowedForNewUsers = false;  // turn off built-in lockout
    options.Lockout.MaxFailedAccessAttempts = int.MaxValue; // disable threshold
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add AutoMapper registration here
builder.Services.AddAutoMapper(
    // Example of how to add multiple assemblies
    typeof(Program).Assembly,
    typeof(UpdateAttendanceRecordcommand).Assembly
);

// Register MediatR (scans your assembly for IRequestHandlers)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(UpdateUserCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateVacationRequestCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(AddUserCommandHandler).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(GetVacationRequestsByUserIdHandler).Assembly);
});


// Configure CORS (Specific to your Blazor client)
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Required for cookies/auth headers
    });
});

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])),
        // Add these for better error handling
        ClockSkew = TimeSpan.Zero, // Remove default 5-minute leeway
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    // Add this for better debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine($"Token validated for: {context.Principal.Identity.Name}");
            return Task.CompletedTask;
        }
    };
});

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireApprovedUser", policy =>
        policy.RequireClaim("IsApproved", "True"));
});

//// ? MailKit setup Email service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// Add MailKit if not already added
builder.Services.AddTransient<SmtpClient>(); // Optional: if you want to inject SmtpClient

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            //sqlOptions.MigrationsAssembly("AttendanceSystem.Auth.API");
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
            sqlOptions.CommandTimeout(60);
        }));

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Attendance System API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "Bearer",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// Register services
builder.Services.AddScoped<IAuthoServicesApi, AuthoServicesApi>();
builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultEndpoints();

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}

app.UseHttpsRedirection();

// CORS must come before Authentication/Authorization
app.UseCors("BlazorClient");


/// Serve static files from "profile_images" folder
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.WebRootPath, "profile_images")),
//    RequestPath = "/profile_images"
//});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Seed roles
        foreach (var role in Enum.GetNames(typeof(Roles)))
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = role,
                    RoleType = Enum.Parse<Roles>(role)
                });
            }
        }

        // Seed admin user
        var adminEmail = "admin@example.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                Name = "Admin",
                IsApproved = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Seeding failed");
    }
}

app.Run();