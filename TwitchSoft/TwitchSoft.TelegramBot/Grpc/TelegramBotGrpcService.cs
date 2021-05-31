using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using TwitchSoft.TelegramBot.MediatR.Models;
using static TelegramBotGrpc;

namespace TwitchSoft.TelegramBot.Grpc
{
    public class TelegramBotGrpcService : TelegramBotGrpcBase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;


        public TelegramBotGrpcService(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator;
            this.mapper = mapper;
        }

        public override async Task<Empty> SendDayDigest(DigestInfoRequest request, ServerCallContext context)
        {
            var umd = mapper.Map<UserMessagesDigest>(request);
            await mediator.Send(umd);
            return new Empty();
        }
    }
}
