using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using TwitchSoft.Shared.Database.Models;

namespace TwitchSoft.Shared.ElasticSearch
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["elasticsearch:url"];
            var defaultIndex = configuration["elasticsearch:index"];

            //var settings = new ConnectionSettings(new Uri(url))
            //    .DefaultIndex(defaultIndex)
            //    .DefaultMappingFor<ChatMessage>(m => m
            //        .Ignore(p => p.IsPublished)
            //        .PropertyName(p => p.UserId, "id")
            //    );

            //var client = new ElasticClient(settings);

            //services.AddSingleton<IElasticClient>(client);
        }
    }
}
