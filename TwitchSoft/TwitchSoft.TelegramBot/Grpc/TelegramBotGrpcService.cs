using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static TelegramBotGrpc;

namespace TwitchSoft.TelegramBot.Grpc
{
    public class TelegramBotGrpcService : TelegramBotGrpcBase
    {
        
        public TelegramBotGrpcService()
        {
        }

        public override async Task<Empty> SentDayDigest(DigestInfoRequest request, ServerCallContext context)
        {
            return new Empty();
        }
    }
}
