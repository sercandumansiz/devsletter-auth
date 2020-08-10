using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Startup.Auth.Entities;
using Startup.Auth.Models;
using Startup.Auth.Provider;
using Startup.Auth.Services;

namespace Startup.Auth
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StartupAuthDatabaseSettings>(
                Configuration.GetSection(nameof(StartupAuthDatabaseSettings)));

            services.AddSingleton<IStartupAuthDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<StartupAuthDatabaseSettings>>().Value);

            services.Configure<ApplicationSettings>(
                Configuration.GetSection(nameof(ApplicationSettings)));

            services.AddSingleton<IApplicationSettings>(sp =>
                sp.GetRequiredService<IOptions<ApplicationSettings>>().Value);


            services.AddTransient<IUserService, UserService>();
            services.AddSingleton<IJwtProvider, JwtProvider>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                           options => options.SetIsOriginAllowed(x => _ = true).AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                       );
            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
