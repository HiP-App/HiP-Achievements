using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PaderbornUniversity.SILab.Hip.Achievements.Core.ReadModel;
using PaderbornUniversity.SILab.Hip.Achievements.Core.WriteModel;
using PaderbornUniversity.SILab.Hip.Achievements.Utility;
using PaderbornUniversity.SILab.Hip.DataStore;
using PaderbornUniversity.SILab.Hip.EventSourcing;
using PaderbornUniversity.SILab.Hip.EventSourcing.EventStoreLlp;
using PaderbornUniversity.SILab.Hip.ThumbnailService;
using PaderbornUniversity.SILab.Hip.Webservice;
using PaderbornUniversity.SILab.Hip.Achievements.Model;
using PaderbornUniversity.SILab.Hip.Webservice.Logging;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo;
using PaderbornUniversity.SILab.Hip.EventSourcing.Mongo.Test;
using PaderbornUniversity.SILab.Hip.EventSourcing.FakeStore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using PaderbornUniversity.SILab.Hip.Achievements.Core;

namespace PaderbornUniversity.SILab.Hip.Achievements.Tests
{
    public class TestStartup
    {
        public TestStartup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "EventStore:Host", "" },
                    { "EventStore:Stream", "test" },
                    { "UploadFiles:Path", "Media" },
                    { "UploadFiles:SupportedFormats:0", "png" }
                })
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            ResourceTypes.Initialize();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<EndpointConfig>(Configuration.GetSection("Endpoints"))
                .Configure<DataStoreConfig>(Configuration.GetSection("Endpoints"))
                .Configure<ThumbnailConfig>(Configuration.GetSection("Endpoints"))
                .Configure<EventStoreConfig>(Configuration.GetSection("EventStore"))
                .Configure<AuthConfig>(Configuration.GetSection("Auth"))
                .Configure<UploadFilesConfig>(Configuration.GetSection("UploadFiles"))
                .Configure<CorsConfig>(Configuration);

            services
                .AddSingleton<IEventStore, FakeEventStore>()
                .AddSingleton<IMongoDbContext, FakeMongoDbContext>()
                .AddSingleton<EventStoreService>()
                .AddSingleton<CacheDatabaseManager>()
                .AddSingleton<InMemoryCache>()
                .AddSingleton<IRoutesClient, FakeRouteClient>()
                .AddSingleton<ThumbnailService.ThumbnailService>()
                .AddSingleton<IDomainIndex, EntityIndex>()
                .AddSingleton<IDomainIndex, ExhibitsVisitedIndex>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var serviceProvider = services.BuildServiceProvider(); // allows us to actually get the configured services
            var authConfig = serviceProvider.GetService<IOptions<AuthConfig>>();

            // Configure authentication
            services
                .AddAuthentication(FakeAuthentication.AuthenticationScheme)
                .AddFakeAuthenticationScheme();

            // Configure authorization
            var domain = authConfig.Value.Authority;
            services.AddAuthorization(options =>
            {
                options.AddPolicy("read:datastore",
                    policy => policy.Requirements.Add(new HasScopeRequirement("read:datastore", domain)));
                options.AddPolicy("write:datastore",
                    policy => policy.Requirements.Add(new HasScopeRequirement("write:datastore", domain)));
                options.AddPolicy("write:cms",
                    policy => policy.Requirements.Add(new HasScopeRequirement("write:cms", domain)));
            });

            services.AddCors();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<CorsConfig> corsConfig, IOptions<EndpointConfig> endpointConfig, IOptions<LoggingConfig> loggingConfig)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"))
                         .AddDebug();

            // CacheDatabaseManager should start up immediately (not only when injected into a controller or
            // something), so we manually request an instance here
            app.ApplicationServices.GetService<CacheDatabaseManager>();

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
