using System;
using API.Data;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env, out IOptions<ApiSettings> apiSettings)
        {
            if (env == null) throw new ArgumentNullException(nameof(env));

            services.Configure<ApiSettings>(config.GetSection("ApiSettings"));

            services.AddScoped<SeedService>();

            services.AddScoped<TokenService>();
            services.AddScoped<LogUserActivity>();

            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            services.AddDbContext(out apiSettings);

            return services;
        }

        private static IServiceCollection AddDbContext(this IServiceCollection services, out IOptions<ApiSettings> apiSettings)
        {
            var scopeFactory = services
                .BuildServiceProvider()
                .GetRequiredService<IServiceScopeFactory>();

            using var scope = scopeFactory.CreateScope();
            apiSettings = scope.ServiceProvider.GetRequiredService<IOptions<ApiSettings>>();

            var settings = apiSettings;
            services.AddDbContext<DataContext>(options =>
                options.UseMySQL(settings.Value.ConnectionString));

            return services;
        }
    }
}