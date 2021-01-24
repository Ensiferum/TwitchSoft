using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.ElasticSearch.Interfaces;
using TwitchSoft.Shared.ElasticSearch.Models;
using TwitchSoft.Shared.Models;
using TwitchSoft.Shared.Services.Models;

namespace TwitchSoft.Shared.ElasticSearch
{
    public class ESService : IESService
    {
        private readonly IElasticClient elasticClient;

        public ESService(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }

        public async Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, int skip = 0, int count = 25)
        {
            var searchResponse = await elasticClient.SearchAsync<ChatMessage>(s => s
                                    .Skip(skip)
                                    .Size(count)
                                    .Query(q => q
                                         .Match(m => m
                                            .Field(c => c.UserId)
                                            .Query(userId.ToString())
                                         )
                                    )
                                    .Sort(s => s
                                        .Descending(_ => _.PostedTime)
                                    )
                                );

            return searchResponse.Documents.Select(_ => new ChatMessageModelForDisplaying()
            {
                UserName = _.UserName,
                Message = _.Message,
                PostedTime = _.PostedTime,
                Channel = _.ChannelName
            }).ToList();
        }

        public async Task<List<ChatMessageModelForDisplaying>> GetMessages(uint userId, DateTime from, int count = 25)
        {
            var searchResponse = await elasticClient.SearchAsync<ChatMessage>(s => s
                                    .Size(count)
                                    .Query(q => q
                                         .Match(m => m
                                            .Field(c => c.UserId)
                                            .Query(userId.ToString())
                                         ) && q.DateRange(d => d
                                            .Field(c => c.PostedTime)
                                            .GreaterThanOrEquals(from)
                                         )
                                    )
                                    .Sort(s => s
                                        .Descending(_ => _.PostedTime)
                                    )
                                );

            return searchResponse.Documents.Select(_ => new ChatMessageModelForDisplaying()
            {
                UserName = _.UserName,
                Message = _.Message,
                PostedTime = _.PostedTime,
                Channel = _.ChannelName
            }).ToList();
        }

        public async Task<List<ChatMessageModelForDisplaying>> SearchMessages(string searchText, int skip, int count)
        {
            var searchResponse = await elasticClient.SearchAsync<ChatMessage>(s => s
                                    .Query(query => query
                                        .Bool(b => b
                                            .Must(q => q
                                                .Wildcard(m => m
                                                    .Field(c => c.Message)
                                                    .Value(searchText)
                                                    )
                                                )
                                            .MustNot(q => 
                                                q.Wildcard(m => m
                                                    .Field(c => c.UserName)
                                                    .Value("*bot"))
                                                || q.Match(m => m
                                                    .Field(c => c.UserName)
                                                    .Query("streamelements"))
                                                || q.Match(m => m
                                                    .Field(c => c.UserName)
                                                    .Query("streamlabs"))
                                            )
                                        )
                                    )
                                    .From(skip)
                                    .Size(count)
                                    .Sort(s => s
                                        .Descending(_ => _.PostedTime)
                                    )
                                );

            return searchResponse.Documents.Select(_ => new ChatMessageModelForDisplaying()
            {
                UserName = _.UserName,
                Message = _.Message,
                PostedTime = _.PostedTime,
                Channel = _.ChannelName
            }).ToList();
        }
    }
}
