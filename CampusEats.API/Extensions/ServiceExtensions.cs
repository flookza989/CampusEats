using CampusEats.API.Configurations;
using CampusEats.Infrastructure.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CampusEats.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Add Infrastructure Layer
            services.AddInfrastructureServices(configuration);

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins",
                    builder =>
                    {
                        builder.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>())
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Add API Controllers
            services.AddControllers()
                   .AddJsonOptions(options =>
                   {
                       options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                   });

            // Add Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CampusEats API",
                    Version = "v1",
                    Description = "API for Campus Food Ordering System",
                    Contact = new OpenApiContact
                    {
                        Name = "Support",
                        Email = "support@campuseats.com"
                    }
                });

                // Include XML Comments
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // Add JWT Authentication
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
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
                        Array.Empty<string>()
                    }
                });

                // Custom Schema IDs
                options.CustomSchemaIds(type => type.FullName);
            });

            // Add JWT Authentication
            services.AddJwtConfiguration(configuration);

            // Add HTTP Context Accessor
            services.AddHttpContextAccessor();

            return services;
        }
    }
}
