using Application;
using Application.Features.Identity.Tokens;
using Application.Wrappers;
using Finbuckle.MultiTenant;
using Infrastructure.Constants;
using Infrastructure.Context;
using Infrastructure.Identity.Auth;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            return services.AddDbContext<TenantDbContext>(options => options
            .UseSqlServer(config.GetConnectionString("DefaultConnection")))
                .AddMultiTenant<ABSchoolTenantInfo>()
                .WithHeaderStrategy(TenancyConstans.TenantIdName)
                .WithClaimStrategy(TenancyConstans.TenantIdName)
                .WithEFCoreStore<TenantDbContext, ABSchoolTenantInfo>()
                .Services
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(config.GetConnectionString("DefaultConnection")))
                .AddTransient<ITenantDbSeeder,TenantDbSeeder>()
                .AddTransient<ApplicationDbSeeder>()
                .AddIdentityService()
                .AddPermissions();
        }
        public static async Task AddDatabaseInitializer(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<ITenantDbSeeder>()
                .InitializeDatabaseAsync(cancellationToken);
        }
        internal static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            return services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders().Services
            .AddScoped<ITokenService, ITokenService>();

        }

        internal static IServiceCollection AddPermissions(this IServiceCollection services)
        {
            return services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
                .AddScoped<IAuthorizationHandler, PermissionAuthorzationHandler>();
        }
        public static JwtSettings GetJwtSettings(this IServiceCollection service,IConfiguration configuration)
        {
            var jwtSettingsConfig = configuration.GetSection(nameof(JwtSettings));
            service.Configure<JwtSettings>(jwtSettingsConfig);
            return jwtSettingsConfig.Get<JwtSettings>();
        }
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
        {
            var secret = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).
            AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = ClaimTypes.Role
                };
                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if(context.Exception is SecurityTokenExpiredException)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("Token Has Expired!"));
                            return context.Response.WriteAsync(result);
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("An error occured while processing your authentication"));
                            return context.Response.WriteAsync(result);
                        }
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(ResponseWrapper.Fail("You are not Authorized"));
                        return context.Response.WriteAsync(result);
                    }
                };
            });
            services.AddAuthorization(options =>{
                foreach(var prop in typeof(SchoolPermission).GetNestedTypes().
                SelectMany(type => type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
                {
                    var propertyValue = prop.GetValue(null);
                    if(propertyValue  is not null)
                    {
                        options.AddPolicy(propertyValue.ToString(), 
                            policy => policy.RequireClaim(ClaimConstants.Permission, propertyValue.ToString()));
                    }
                }
            });
            return services;
        }
        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
        {
            return app
                .UseMultiTenant();
        }
    }
}
