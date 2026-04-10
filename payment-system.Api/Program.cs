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

// ===== DATABASE CONTEXT =====
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});

// ===== JWT AUTHENTICATION CONFIGURATION (Faz 4) =====
// appsettings.json dosyasındaki ayarları okuyoruz
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

// ===== DEPENDENCY INJECTION =====
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerDocumentation(); // Not: Buraya JWT desteği eklemen gerekecek
builder.Services.AddControllers();

var app = builder.Build();

// ===== MIDDLEWARE =====
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

// SIRALAMA ÇOK ÖNEMLİ:
app.UseAuthentication(); // 1. Kimsin? (Yeni eklendi)
app.UseAuthorization();  // 2. Bu işlemi yapmaya yetkin var mı?

app.MapControllers();

app.Run();