using Microsoft.EntityFrameworkCore;
using MicroCredit.Data;
using MicroCredit.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using MicroCredit.Controllers;
using Microsoft.AspNetCore.HttpOverrides;
using System;


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
            // Configure services
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllersWithViews();
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });
            services.AddScoped<UserController>();  // Add UserController to DI container
            services.AddScoped<LoanService>();
            services.AddSingleton<TwilioService>();

            // Add services to the container.
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Enable HSTS in non-development environments
            }

            app.UseHttpsRedirection(); // Force HTTPS redirection
            app.UseRouting();

            // Enable CORS
            app.UseCors("AllowAll");

            app.UseEndpoints(endpoints =>
            {
                // Automatically maps controller routes
                endpoints.MapControllers();

                // Optionally, you can add a default health check route
                endpoints.MapGet("/", () => Results.Ok("MicroCredit API is running."));
            });
        }

    }
}
