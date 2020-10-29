using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitchSoft.TwitchBotOrchestrator.Hubs
{
    public interface IOrchestrationClient
    {
        Task JoinChannelsCommand(IEnumerable<string> channels);
    }
}
