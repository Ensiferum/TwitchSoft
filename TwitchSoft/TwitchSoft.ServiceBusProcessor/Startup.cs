using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared;
using TwitchSoft.Shared.ElasticSearch;
using TwitchSoft.Shared.Redis;
using TwitchSoft.Shared.Services.TwitchApi;

namespace TwitchSoft.ServiceBusProcessor
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
            services.ConfigureShared();

            services.AddServiceBusProcessors(Configuration);

            services.AddCache(Configuration);
            services.AddElasticSearch(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

        }
    }
}