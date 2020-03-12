using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SenseNet.Configuration;
using SenseNet.ContentRepository;
using SenseNet.ContentRepository.Security;
using SenseNet.ContentRepository.Storage.Data.MsSqlClient;
using SenseNet.Diagnostics;
using SenseNet.OData;
using SenseNet.Search.Lucene29;
using SenseNet.Security.EFCSecurityStore;
using SenseNet.Security.Messaging.RabbitMQ;

namespace SenseNet.Web.Api.Sql.IndexServer.Admin
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
            services.AddControllers();
            services.Configure<SenseNetEnvironment>(Configuration.GetSection("sensenet"));

            services.AddHttpsRedirection(options =>
            {
                var defaultPort = 443;
                var aspPort = 0;
                var aspPortSrc = System.Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT");
                if (!string.IsNullOrEmpty(aspPortSrc))
                    int.TryParse(aspPortSrc, out aspPort);
                options.HttpsPort = aspPort > 0 ? aspPort : defaultPort;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            // [sensenet]: OData
            app.UseSenseNetOdata();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        internal static RepositoryBuilder GetRepositoryBuilder(IConfiguration configuration, string currentDirectory)
        {
            // assemble a SQL-specific repository

            var repositoryBuilder = new RepositoryBuilder()
                .UseConfiguration(configuration)
                .UseLogger(new SnFileSystemEventLogger())
                .UseTracer(new SnFileSystemTracer())
                .UseAccessProvider(new UserAccessProvider())
                .UseDataProvider(new MsSqlDataProvider())
                .UseSecurityDataProvider(new EFCSecurityDataProvider(connectionString: ConnectionStrings.ConnectionString))
                .UseSecurityMessageProvider(new RabbitMQMessageProvider())
                .UseLucene29CentralizedSearchEngine(
                    new NetTcpBinding(),
                    new EndpointAddress(configuration["sensenet:search:service:address"]))
                .StartWorkflowEngine(false)
                .DisableNodeObservers()
                .UseTraceCategories("Event", "Custom", "System") as RepositoryBuilder;

            Providers.Instance.PropertyCollector = new EventPropertyCollector();

            return repositoryBuilder;
        }

    }
}
