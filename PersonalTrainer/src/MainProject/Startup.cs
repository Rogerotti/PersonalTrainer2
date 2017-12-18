using Framework.DataBaseContext;
using Framework.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MainProject.ViewNavigator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace MainProject
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        private readonly IHostingEnvironment hostingEnviroment;

        public Startup(IHostingEnvironment hostingEnviroment)
        {
            this.hostingEnviroment = hostingEnviroment;
            var builder = new ConfigurationBuilder();

            if (hostingEnviroment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }
                
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            var connection = @"Server=(localdb)\MSSQLLocalDB;Database=PersonalTrainer;Trusted_Connection=True;";
            services.AddDbContext<DefaultContext>(
                options => options.UseSqlServer(connection, b => b.MigrationsAssembly("MainProject"))
                , ServiceLifetime.Singleton);

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            });


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IUserManagement, UserManagement>();
            services.AddScoped<IProductManagement, ProductManagement>();
            services.AddScoped<IUserGoalsManagement, UserGoalsManagement>();
            // services.AddLocalization(options => options.ResourcesPath = "Resources");
            var mvcBuilder = services.AddMvc()
                .AddJsonOptions(jsonOptions =>
                {
                    jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    jsonOptions.SerializerSettings.DateFormatString = "yy-mm-dd";
                }).AddMvcOptions(x => { x.Filters.Add(new RequireHttpsAttribute()); });

            IList<ModuleInfo> modules = GetModules(hostingEnviroment);

            foreach (var module in modules)
                mvcBuilder.AddApplicationPart(module.Assembly);

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new ModuleViewLocationExpander());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// Pozyskuje moduły aplikacji
        /// </summary>
        /// <param name="hostingEnviroment"></param>
        /// <returns></returns>
        private List<ModuleInfo> GetModules(IHostingEnvironment hostingEnviroment)
        {
            var modules = new List<ModuleInfo>();
            var moduleRootFolder = new DirectoryInfo(Path.Combine(hostingEnviroment.ContentRootPath, "Modules"));
            var moduleFolders = moduleRootFolder.GetDirectories();
            foreach (var moduleFolder in moduleFolders)
            {
                var binFolder = new DirectoryInfo(Path.Combine(moduleFolder.FullName, "bin"));
                if (!binFolder.Exists)
                    continue;

                foreach (var file in binFolder.GetFileSystemInfos(moduleFolder.Name + ".dll", SearchOption.AllDirectories))
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
                    }
                    catch (FileLoadException ex)
                    {
                        if (ex.Message == "Assembly with same name is already loaded")
                            assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(file.Name)));
                        else
                            throw;
                    }
                    if (assembly.FullName.Contains(moduleFolder.Name))
                    {
                        if (!modules.Any(x => x.Name.Equals(moduleFolder.Name)))
                            modules.Add(new ModuleInfo
                            {
                                Name = moduleFolder.Name,
                                Assembly = assembly
                            });
                    }
                }
            }
            return modules;
        }
    }
}