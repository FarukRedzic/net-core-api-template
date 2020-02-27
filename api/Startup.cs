using System;
using System.Net.Http;
using System.Security.Claims;
using api.database.DbContexts.TemplateDb;
using api.model.Interfaces.Factory;
using api.model.Interfaces.HttpClients;
using api.repository.EntityFramework.Implementations;
using api.repository.EntityFramework.Interfaces;
using api.service.Factory;
using api.service.HttpClients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Swashbuckle.AspNetCore.Swagger;

namespace api
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
            services.AddCors();
            services.AddAuthentication("Bearer")
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = Configuration.GetSection("AppSettings:OIDC:AUTHORITY").Value; //Identity Server Url
                        options.ApiName = Configuration.GetSection("AppSettings:OIDC:RESOURCE_API_NAME").Value;
                        options.RequireHttpsMetadata = true;
                        options.RoleClaimType = ClaimTypes.Role; // Enable role authorization
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Resource API",
                    Version = "v1"
                });
            });

            //-------------------------------------------
            // Factory registration
            //-------------------------------------------
            services.AddScoped<ITemplateServiceFactory, TemplateServiceFactory>();

            //-------------------------------------------
            // DbContext registration
            //-------------------------------------------
            services.AddDbContext<TemplateDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(nameof(TemplateDbContext)))
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            //-------------------------------------------
            // HttpClientFactory registration
            //-------------------------------------------
            services.AddHttpClient<ITemplateHttpClientService, TemplateHttpClientService>()
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5)) //HttpClient Lifetime
                    .AddPolicyHandler(GetRetryPolicy()); // Custom policy using Polly

            //-------------------------------------------
            // Generic Repository registration
            //-------------------------------------------
            services.AddScoped(typeof(IEFRepository<>), typeof(EFRepository<>));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        /// <summary>
        /// Conflict example.
        /// In this case, the policy is configured to try five times with an exponential retry, starting at two seconds.
        /// </summary>
        /// <returns></returns>
        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Conflict)
                .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // Specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resource API");
            });

            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());
            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
