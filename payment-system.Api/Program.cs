using Microsoft.EntityFrameworkCore;
using payment_system.Api.Endpoints;
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

// ===== DEPENDENCY INJECTION - REPOSITORIES =====
builder.Services.AddScoped<IAccountRepository, AccountRepository>();           // ✅ YENİ
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// ===== DEPENDENCY INJECTION - SERVICES =====
builder.Services.AddScoped<IAccountService, AccountService>();                // ✅ YENİ
builder.Services.AddScoped<ITransactionService, TransactionService>();

// ===== CONTROLLERS VE ENDPOINTS =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== MIDDLEWARE =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

// ===== ENDPOINT MAPPING =====
app.MapAccountEndpoints();                                                      // ✅ YENİ
app.MapTransactionEndpoints();

app.Run();