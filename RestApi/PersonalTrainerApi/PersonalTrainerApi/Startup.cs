using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using PersonalTrainerApi.Model.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using PersonalTrainerApi.Services;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Framework.Model;

namespace PersonalTrainerApi
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
            services.AddMvc()
                .AddMvcOptions(x => { x.Filters.Add(new RequireHttpsAttribute()); });

            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<DefaultContext>();
            /*
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = "http://localhost:5001/";
                    options.Authority = "http://localhost:5000/";
                });
            /*
            services.AddIdentity<IdentityUser,IdentityRole>()
             .AddEntityFrameworkStores<DefaultContext>()
             .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 3;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", p => p.RequireClaim("user"));
            });
 */
            // Ustawienia podstawowych informacji na temat swaggera
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Personal Trainer Api", Description = "v1" });
            });

            // Ustawienie serwisów
            services.AddSingleton<IUserManagement, UserManagement>();
            services.AddSingleton<IProductManagement, ProductManagement>();
            

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
            /*
            app.Run(async (context) =>
            {
                string not = "Nie znaleziono adresu";
                var byteArray = Encoding.UTF8.GetBytes(not);
                await context.Response.Body.WriteAsync(byteArray, 0, byteArray.Length);
            });'*/
        }
    }
}
