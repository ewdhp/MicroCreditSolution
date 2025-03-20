using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MicroCredit.Middleware;
using MicroCredit.Services;
using MicroCredit.ModelBinders;
using System.Text;
using MicroCredit.Data;
using System.Net.Http;
using System;
using Microsoft.Extensions.Logging;
using MicroCredit.Logging;
using MicroCredit.Controllers;
using MicroCredit.Interfaces;

namespace MicroCredit
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register services
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<JwtTokenService>();
            services.AddScoped<FingerprintService>();

            //Loan service
            services.AddScoped<LoanService>();

            // Register phase services
            services.AddScoped<IPhaseFactory, PhaseFactory>();
            services.AddScoped<PhaseController>();
            services.AddSingleton<InitialService>();
            services.AddSingleton<ApprovalService>();
            services.AddSingleton<PayService>();


            // Register IJwtTokenService
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Http context accessor
            services.AddHttpContextAccessor();

            // Register IUserContextService
            services.AddScoped<IUserContextService, UserContextService>();

            // Register HttpClient
            services.AddHttpClient();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new PhaseRequestModelBinderProvider());
            });

            var jwtKey = Configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("Jwt:Key", "JWT Key is not configured.");
            }
            var key = Encoding.ASCII.GetBytes(jwtKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                x.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy", policy => policy.RequireClaim("UserId"));
            });

            // Clear default logging providers and register the custom logger provider
            services.AddLogging(loggingBuilder =>
            {

                loggingBuilder.AddProvider(new LoggerCustomProvider(LogLevel.Information));
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}