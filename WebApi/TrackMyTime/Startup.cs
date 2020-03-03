using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TrackMyTime.Services;

namespace TrackMyTime
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
            services.AddApiVersioning(opt =>
            {
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
            });

            services.AddControllers();

            services.AddSingleton<IMyTimeRepo>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("AzureConfig")).GetAwaiter().GetResult());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Track My Time API",
                    Version = "v1",
                    Description = "Allows you to get and post start and end times that you want to be tracked.",
                    Contact = new OpenApiContact
                    {
                        Name = "Richard Todosichuk",
                        Email = "rtodosic@hotmail.com",
                        Url = new Uri("http://google.com")
                    }
                });
            });
        }

        /// <summary>
        /// Creates a Cosmos DB database and a container with the specified partition key.
        /// </summary>
        /// <returns></returns>
        private static async Task<MyTimeRepo> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string account = configurationSection.GetSection("CosmosAccount").Value;
            string key = configurationSection.GetSection("CosmosKey").Value;

            var clientBuilder = new CosmosClientBuilder(account, key);
            var client = clientBuilder.WithConnectionModeDirect().Build();
            var database = await client.CreateDatabaseIfNotExistsAsync("tmt").ConfigureAwait(false);

            var containerProps = new ContainerProperties
            {
                Id = "Times", // container name
                PartitionKeyPath = "/userId"
            };
            containerProps.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
            containerProps.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/\"notes\"/?" });
            containerProps.IndexingPolicy.CompositeIndexes.Add(new Collection<CompositePath>
            {
                new CompositePath {Path = "/userId", Order=CompositePathSortOrder.Ascending},
                new CompositePath {Path = "/timGroup", Order=CompositePathSortOrder.Ascending}
            });
            containerProps.IndexingPolicy.CompositeIndexes.Add(new Collection<CompositePath>
            {
                new CompositePath {Path = "/userId", Order=CompositePathSortOrder.Ascending},
                new CompositePath {Path = "/timGroup", Order=CompositePathSortOrder.Ascending},
                new CompositePath {Path = "/start", Order=CompositePathSortOrder.Descending}
            });
            var container = await database.Database.CreateContainerIfNotExistsAsync(containerProps, 400).ConfigureAwait(false);

            var myTimeRepo = new MyTimeRepo(client.GetContainer(database.Database.Id, container.Container.Id));
            return myTimeRepo;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Track My Time API");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
