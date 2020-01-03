using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using static TelegramBotGrpc;

namespace TwitchSoft.TelegramBot.Grpc
{
    public class TelegramBotGrpcService : TelegramBotGrpcBase
    {
        private readonly TelegramBot telegramBot;


        public TelegramBotGrpcService(TelegramBot telegramBot)
        {
            this.telegramBot = telegramBot;
        }

        public override async Task<Empty> SentDayDigest(DigestInfoRequest request, ServerCallContext context)
        {
            await telegramBot.SentDigest(request);
            return new Empty();
        }
    }
}
