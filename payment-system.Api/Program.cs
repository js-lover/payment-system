using Microsoft.EntityFrameworkCore;
using payment_system.Api.Extensions;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Implementations;
using payment_system.Application.Services.Interfaces;
using payment_system.Infrastructure.Persistence.Contexts;
using payment_system.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database Context Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});

// JWT Authentication Configuration
// Read settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Dependency Injection Configuration
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// AutoMapper Configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddSwaggerDocumentation(); // Note: JWT support is configured via JwtBearerDefaults
builder.Services.AddControllers();

var app = builder.Build();

// Middleware Configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment System API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

// Middleware execution order is critical:
app.UseAuthentication(); // 1. Identify the user (Who are you?)
app.UseAuthorization();  // 2. Check permissions (Are you allowed to do this?)

app.MapControllers();

app.Run();