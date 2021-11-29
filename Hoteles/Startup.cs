using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Hoteles.Configurations;
using Hoteles.Contracs;
using Hoteles.Data.Context;
using Hoteles.Filters.Action;
using Hoteles.Repository;
using Hoteles.Services;
using Hoteles.Services.contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Hoteles
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(o => 
                o.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );
            /*services.AddAuthentication();*/
            services.ConfigureIdentity();
            services.ConfigureJWT(Configuration);
            services.AddCors(c =>
            {
                c.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
            
            //Automapper
            services.AddAutoMapper(typeof(MapperInitialize));
            services.AddTransient<IUnitOFWork, UnitOfWork>();
            services.AddScoped<IAuthManager, AuthManager>();
            services.AddHttpContextAccessor();
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
                
            services.AddTransient<ValidateModelAttribute>();
            services.AddTransient<ValidateHotelExistsAttribute>();
            services.AddTransient<ValidateCountryExistsAttribute>();
            services.AddTransient<ValidationModel>();
            services.ConfigureRateLimitingOptions();
            /*services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();*/
            services.AddMemoryCache();
            services.ConfigureHttpCacheHeaders();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hotels by Country", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "Hotels by Country", Version = "v2" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme()
                        {
                            Reference = new OpenApiReference()
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });

            });
            services.AddControllers( config =>
                {
                    config.CacheProfiles.Add("120secondsDuration", new CacheProfile()
                    {
                        Duration = 120
                    });
                }
                
            ).AddNewtonsoftJson(
                options => options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.ConfigureVersioning();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Listig API v1");
                    s.SwaggerEndpoint("/swagger/v2/swagger.json", "Hotel Listig API v2");
                });
            }
            app.ConfigureExceptionHandler();
            //app.UseHttpsRedirection();
            //app.UseIpRateLimiting();
            app.UseCors("CorsPolicy");
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}