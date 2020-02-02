﻿using Dapper;
using Microsoft.Extensions.DependencyInjection;
using TwitchSoft.Shared.Services.Repository;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.Shared
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureShared(this IServiceCollection services)
        {
            SqlMapper.AddTypeMap(typeof(uint), System.Data.DbType.Int64);

            services.AddScoped<IRepository, Repository>();
        }
    }
}