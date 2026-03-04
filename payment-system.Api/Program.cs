using Microsoft.EntityFrameworkCore;
using payment_system.Infrastructure.Persistence.Contexts; // reference to the DbContext


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Use SQLite database
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Enable Lazy Loading
    options.UseLazyLoadingProxies();
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();