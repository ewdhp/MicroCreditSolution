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
using System.Text;
using MicroCredit.Data;
using System.Net.Http;
using System;
using MicroCredit.Services.Auth;

namespace MicroCredit
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register services
            services.AddDbContext<UDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<JwtTokenService>();
            services.AddScoped<FingerprintService>();

            // Register TokenValidationService
            services.AddSingleton<TokenValidationService>();

            // Register other services as needed
            services.AddSingleton<TokenManagementService>();
            services.AddSingleton<AuditLoggingService>();
            services.AddSingleton<ErrorHandlingService>();
            services.AddSingleton<RBACService>();

            // Register AuthModule
            services.AddSingleton<AuthModule>();

            // Loan service
            services.AddScoped<ILoanService, LoanService>();

            // Register phase services
            services.AddScoped<PhaseService>();

            // Register Loan services
            services.AddScoped<LoanService>();

            // Register payment service
            services.AddScoped<PayService>();

            // Register IJwtTokenService
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            // Register IUCService and IHttpContextAccessor
            services.AddHttpContextAccessor();
            services.AddScoped<IUCService, UserContextService>();

            // Register HttpClient
            services.AddHttpClient();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.WithOrigins("http://74.208.246.177:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddControllers();

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
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero // Disable clock skew
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

            // Register FileWatcherService and CleanupBackgroundService
            services.AddSingleton<FileWatcherService>();
            services.AddHostedService<CleanupBackgroundService>();
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

            // Enable WebSocket support
            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            app.UseWebSockets(webSocketOptions);

            // Add JWT authentication middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Add JwtMiddleware for HTTP requests
            app.UseMiddleware<JwtMiddleware>();

            // Add WebSocketAuthMiddleware for WebSocket requests
            app.UseMiddleware<UnifiedAuth>();

            // Lock middleware to prevent multiple requests from the same user
            app.UseMiddleware<UserRequestLockMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}