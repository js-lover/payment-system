using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using payment_system.Application.Repositories;
using payment_system.Application.Services.Interfaces;
using payment_system.Application.Services.Implementations;
using payment_system.Infrastructure.Persistence.Contexts;
using payment_system.Infrastructure.Repositories;
using payment_system.Infrastructure.Security;

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
            services.AddScoped<IUserRepository, UserRepository>();

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
            services.AddScoped<IPasswordService, BCryptPasswordService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();


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


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
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