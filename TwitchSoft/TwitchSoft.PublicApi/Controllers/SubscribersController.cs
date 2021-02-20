using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Repository.Interfaces;

namespace TwitchSoft.PublicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribersController : ControllerBase
    {
        private readonly ILogger<SubscribersController> _logger;
        private readonly ISubscriptionRepository subscriptionRepository;

        public SubscribersController(
            ILogger<SubscribersController> logger, 
            ISubscriptionRepository subscriptionRepository)
        {
            _logger = logger;
            this.subscriptionRepository = subscriptionRepository;
        }

        [HttpGet]
        [Route("count")]
        public async Task<CountResult> Count(CountParameters countParameters)
        {
            var ( channel, from, to ) = countParameters;
            var count = await subscriptionRepository.GetSubscribersCountFor(channel, from, to);

            return new CountResult(count);
        }
    }

    public record CountParameters(string Channel, DateTime From, DateTime To);
    public record CountResult(int Count);
}
