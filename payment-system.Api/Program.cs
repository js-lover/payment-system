using Microsoft.EntityFrameworkCore;
using payment_system.Api.Extensions;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Implementations;
using payment_system.Application.Services.Interfaces;
using payment_system.Infrastructure.Persistence.Contexts;
using payment_system.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ===== DATABASE CONTEXT =====
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseLazyLoadingProxies();
});

// ===== DEPENDENCY INJECTION =====
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
builder.Services.AddSwaggerDocumentation();
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
app.UseAuthorization();
app.MapControllers();

app.Run();