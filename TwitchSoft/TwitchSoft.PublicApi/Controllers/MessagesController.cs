using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitchSoft.Shared.Services.Models;
using TwitchSoft.Shared.Services.Repository.Interfaces;
using TwitchSoft.Shared.Services.TwitchApi;

namespace TwitchSoft.PublicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ILogger<MessagesController> _logger;
        private readonly IMessageRepository messageRepository;
        private readonly ITwitchApiService twitchApiService;

        public MessagesController(
            ILogger<MessagesController> logger,
            IMessageRepository messageRepository, 
            ITwitchApiService twitchApiService)
        {
            _logger = logger;
            this.messageRepository = messageRepository;
            this.twitchApiService = twitchApiService;
        }

        [HttpGet]
        public async Task<UserMessagesResult> UserMessages(string user, int skip = 0, int count = 25)
        {
            var userInfo = await twitchApiService.GetChannelByName(user);
            var messages = await messageRepository.GetMessages(uint.Parse(userInfo.Id), skip, count + 1);

            var messagesToReturn = messages.Take(count);
            var isMoreExist = messages.Count > count;

            return new UserMessagesResult(messagesToReturn, isMoreExist);
        }
    }

    public record UserMessagesResult(IEnumerable<ChatMessageModelForDisplaying> Messages, bool IsMoreExist);
}
