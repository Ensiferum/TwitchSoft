using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TwitchSoft.Shared.Services.Repository
{
    public abstract class BaseRepository
    {
        internal readonly IConfiguration configuration;
        internal readonly ILogger<BaseRepository> logger;

        internal string ConnectionString => configuration.GetConnectionString("TwitchDb");

        public BaseRepository(
            IConfiguration configuration, 
            ILogger<BaseRepository> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }
    }
}
