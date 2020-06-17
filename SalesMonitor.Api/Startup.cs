using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;
using SalesMonitor.Api.Data;
using System;

namespace SalesMonitor.Api
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
            services.Configure<RepositoryConfiguration>(Configuration.GetSection(
                RepositoryConfiguration.ConfigSection));
            services.AddControllers()
                .AddJsonOptions(configure =>
                {
                    configure.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                });

            var repositoryConfiguration = Configuration.GetSection(RepositoryConfiguration.ConfigSection)
                .Get<RepositoryConfiguration>();

            switch (repositoryConfiguration.RepositoryKind)
            {
                case RepositoryKind.InMemory:
                    services.AddSingleton<IRepository, RepositoryInMemory>();
                    break;
                case RepositoryKind.CosmosDb:
                    services.AddScoped<IRepository, RepositoryCosmosDb>();
                    break;
                default:
                    throw new NotSupportedException($"The value " +
                        $"{repositoryConfiguration.RepositoryKind} does not represent a known repository.");
            }

            if (repositoryConfiguration.RepositoryKind == RepositoryKind.CosmosDb)
            {
                services.AddDbContext<SalesMonitorContext>(options =>
                    options.UseCosmos(
                        accountEndpoint: repositoryConfiguration.CosmosDbEndpointUri,
                        accountKey: repositoryConfiguration.CosmosDbApiKey,
                        databaseName: "SalesMonitor"));
            }

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "SalesMonitor";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Valentin Rock",
                        Email = string.Empty,
                        Url = "https://github.com/Lupin1st/sales-monitor"
                    };
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());

            app.UseOpenApi();

            app.UseSwaggerUi3();
        }
    }
}
