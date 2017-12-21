using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalTrainerApi.Model.Database.Context;
using Microsoft.EntityFrameworkCore;
using PersonalTrainerApi.Services;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PersonalTrainerApi.Model.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace PersonalTrainerApi
{
    public class Startup
    {

        public IConfiguration Configuration { get; }
        private readonly IHostingEnvironment hostingEnvironment;

        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            this.hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services
                .AddMemoryCache();
           
            if (hostingEnvironment.IsDevelopment())
            {
                services.AddMvc(opts =>
                {
                    // opts.Filters.Add(new AllowAnonymousFilter());
                });
            }
            else
            {
                services.AddMvc();
            }

            #region Kestrel
            services.Configure<KestrelServerOptions>(options =>
            {
                options.Listen(IPAddress.Any, 5000);
                if (hostingEnvironment.IsDevelopment())
                {
                    options.Listen(IPAddress.Any, 5002);
                }
                // Usunięcie nagłówka "Server:Kestrel" z odpowiedzi HTTP.
                options.AddServerHeader = false;
            });
            #endregion Kestrel

            // Ustawienia podstawowych informacji na temat swaggera
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Personal Trainer Api", Description = "v1" });
            });

            // Ustawienie serwisów
            services.AddSingleton<IUserManagement, UserManagement>();
            services.AddSingleton<IProductManagement, ProductManagement>();
            services.AddSingleton<IAuthorizationManagement, AuthorizationManagement>();
            string domain = "https://personal-trainer.com/";
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.Authority = domain;
                options.Audience = "https://personal-trainer";
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("user", policy => policy.Requirements.Add(new HasScopeRequirement("user", domain)));
                options.AddPolicy("admin", policy => policy.Requirements.Add(new HasScopeRequirement("admin", domain)));
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

            // Ustawienie połączenia z bazą danych
            services.AddDbContext<DefaultContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Singleton);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseAuthentication();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Personaltrainerppi");
            });

        }
    }
}
