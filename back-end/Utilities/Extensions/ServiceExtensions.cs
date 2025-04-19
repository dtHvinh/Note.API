using back_end.Attributes;
using back_end.Data;
using back_end.Model;
using back_end.Services;
using back_end.Utilities.Options;
using back_end.Utilities.Provider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text;
using static back_end.Utilities.Names.Constants;

namespace back_end.Utilities.Extensions;

public static class ServiceExtensions
{
    private static IConfiguration Configuration { get; set; } = default!;

    public static IServiceCollection WithConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        Configuration = configuration;

        return services;
    }

    public static IServiceCollection ConfigureLogging(this IServiceCollection services)
    {
        return services.AddSerilog(e =>
          {
              e.WriteTo.Console();

              e.WriteTo.File("D:\\dev\\myproject\\a\\back-end\\back-end\\Logs", rollingInterval: RollingInterval.Day);
              e.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
              .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning);
          });
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services)
    {
        return services.Configure<JwtOptions>(
            Configuration.GetSection("JwtOptions"));
    }

    public static IServiceCollection ConfigureAuth(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtOptions:SecretKey"]!)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services
        .AddSingleton(cf =>
        {
            var key = Configuration["Supabase:Key"];
            var url = Configuration["Supabase:Url"];

            ArgumentNullException.ThrowIfNull(key, nameof(key));
            ArgumentNullException.ThrowIfNull(url, nameof(url));

            var storage = new StorageService(url, key);

            storage.InitializeAsync().GetAwaiter().GetResult();

            return storage;
        })
        .AddScoped<JwtTokenProvider>()
        .AddScoped<TokenValidationService>();

        Assembly.GetCallingAssembly().DefinedTypes
            .Where(x => x.GetCustomAttribute<DependencyAttribute>() != null)
            .ToList()
            .ForEach(type =>
            {
                var attr = type.GetCustomAttribute<DependencyAttribute>();

                if (attr!.BaseType != null)
                {
                    services.Add(new ServiceDescriptor(attr.BaseType, type, attr.Lifetime));
                }
                else
                {
                    services.Add(new ServiceDescriptor(type, type, attr.Lifetime));
                }
            });


        return services;
    }

    public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
    {
        services
            .AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(Configuration.GetConnectionString("ApplicationDbContext"));
            })
            .AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }

    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient(HttpClientNames.GoogleUserInfo, client =>
        {
            client.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v2/userinfo");
        });

        return services;
    }
}
