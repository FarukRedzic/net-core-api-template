using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using identity.Configuration;
using identity.Data.DbContexts;
using identity.Data.Models;
using identity.Interfaces;
using identity.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace identity
{
    public class Startup
    {
        #region Fields
        IConfiguration _config; 
        #endregion

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            string userIdentityConnectionString = _config.GetConnectionString("IdentityUserDbContext");
            string identityConfigurationDbContext = _config.GetConnectionString("IdentityConfigurationDbContext");
            string migrationAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<IdentityDbContext>(options =>
             options.UseSqlServer(userIdentityConnectionString, o =>
                   o.MigrationsAssembly(migrationAssembly)));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityDbContext>()
                    .AddDefaultTokenProviders();

            services.AddIdentityServer()
                    .AddDeveloperSigningCredential() // TODO: Use real certificate in prod environment
                    .AddAspNetIdentity<ApplicationUser>()
                    //Configuration Store: clients and resources
                    .AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = db =>
                        db.UseSqlServer(identityConfigurationDbContext,
                            sql => sql.MigrationsAssembly(migrationAssembly));
                    })
                    //Operational Store: tokens, codes etc.
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = db =>
                        db.UseSqlServer(identityConfigurationDbContext,
                            sql => sql.MigrationsAssembly(migrationAssembly));
                    })
                    .AddProfileService<IdentityProfileService>(); // custom claims 

            services.AddScoped<IIdentityUserService<ApplicationUser>, IdentityUserService<ApplicationUser>>();
            services.AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            await InitializeIdentitySrvDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer();
        }

        private async Task InitializeIdentitySrvDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<IdentityDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var configurationCtx = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configurationCtx.Database.Migrate();

                bool refreshConfiguration;
                Boolean.TryParse(_config["RefreshIdentityConfiguration"], out refreshConfiguration);
                if (refreshConfiguration) // re-create Identity Configuration in database from config file
                {
                    var dbClients = configurationCtx.Clients.ToList();
                    var dbApiResources = configurationCtx.ApiResources.ToList();
                    var dbIdentityResources = configurationCtx.IdentityResources.ToList();

                    configurationCtx.Clients.RemoveRange(dbClients);
                    configurationCtx.ApiResources.RemoveRange(dbApiResources);
                    configurationCtx.IdentityResources.RemoveRange(dbIdentityResources);
                    configurationCtx.SaveChanges();
                }

                if (!configurationCtx.Clients.Any())
                {
                    //Seed clients from Identity configuration
                    foreach (var client in IdentityConfig.GetClients())
                        configurationCtx.Clients.Add(client.ToEntity());
                }

                if (!configurationCtx.ApiResources.Any())
                {
                    //Seed api resources from Identity configuration
                    foreach (var apiResource in IdentityConfig.GetApiResources())
                        configurationCtx.ApiResources.Add(apiResource.ToEntity());
                }

                if (!configurationCtx.IdentityResources.Any())
                {
                    //Seed identity resources from Identity configuration
                    foreach (var identityResource in IdentityConfig.GetIdentityResources())
                        configurationCtx.IdentityResources.Add(identityResource.ToEntity());
                }

                configurationCtx.SaveChanges();

                // seed the Identity Roles 
                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!roleManager.Roles.Any(r => r.Name.Equals("admin", StringComparison.OrdinalIgnoreCase)))
                    await roleManager.CreateAsync(new IdentityRole("admin"));
            }
        }
    }
}
