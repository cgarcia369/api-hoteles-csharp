using Hoteles.Data.Context;
using Hoteles.Data.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Hoteles.Controllers;
using Hoteles.Data.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Hoteles.Services
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection service)
        {
            var builder = service.AddIdentityCore<ApiUser>(r => r.User.RequireUniqueEmail = true);
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), service);
            builder.AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();
        }
        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),

                    };
                });
        }
        public static void ConfigureExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseExceptionHandler(error =>
            {
                error.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        Log.Error($"Something Went Wrong in the {contextFeature.Error}");
                        await context.Response.WriteAsync(new Error()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error. Please Try Again later"
                        }.ToString());
                    }
                });
            });
        }
        public static void ConfigureVersioning(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                options.Conventions.Controller<CountryController>().HasApiVersion(new ApiVersion(1, 0));
                options.Conventions.Controller<CountryV2Controller>().HasDeprecatedApiVersion(new ApiVersion(2, 0));
            });
        }
        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
                (expirations) =>
                {
                    expirations.MaxAge = 120;
                    expirations.CacheLocation = CacheLocation.Private;
                },
                (validationsOpt) =>
                {
                    validationsOpt.MustRevalidate = true;
                });
        }
        public static void ConfigureRateLimitingOptions(this IServiceCollection service)
        {
            var rateLimitRules = new List<RateLimitRule>()
            {
                new RateLimitRule()
                {
                    Endpoint = "+",
                    Limit = 3,
                    Period = "5m"
                }
            };
            service.Configure<IpRateLimitOptions>(options =>
            {
                service.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
                service.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
                service.AddSingleton<IRateLimitConfiguration,RateLimitConfiguration>();
            });
        }
    

    }
}
