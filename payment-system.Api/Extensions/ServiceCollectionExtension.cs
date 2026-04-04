using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Services.Implementations;
using payment_system.Infrastructure.Persistence.Contexts;
using payment_system.Infrastructure.Repositories;

namespace payment_system.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Database context'i kayıt et
        /// </summary>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                options.UseLazyLoadingProxies();
            });

            return services;
        }

        /// <summary>
        /// Repository'leri DI container'a kayıt et
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            return services;
        }

        /// <summary>
        /// Service'leri DI container'a kayıt et
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ICustomerService, CustomerService>();

            return services;
        }

        /// <summary>
        /// Swagger/OpenAPI yapılandırması
        /// </summary>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Payment System API",
                    Version = "v1.0",
                    Description = "Ödeme sistemi API'si - Transaction ve Account yönetimi",
                    Contact = new OpenApiContact
                    {
                        Name = "Payment System Team",
                        Email = "support@paymentsystem.com"
                    }
                });

                // XML comments
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }
    }
}