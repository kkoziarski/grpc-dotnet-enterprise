using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Dotnet.Notifications.Server.Domain.Commands;
using Grpc.Dotnet.Shared.Helpers.Rpc.Server;
using Grpc.Dotnet.Notifications.V1;
using Microsoft.Extensions.Logging;

namespace Grpc.Dotnet.Notifications.Server
{
    public class NotificationServiceV1 : NotificationService.NotificationServiceBase
    {
        private readonly MessageOrchestrator messageOrchestrator;
        private readonly ILogger<NotificationServiceV1> logger;

        public NotificationServiceV1(MessageOrchestrator messageOrchestrator, ILogger<NotificationServiceV1> logger)
        {
            this.messageOrchestrator = messageOrchestrator;
            this.logger = logger;
        }

        public override async Task<Empty> SendEmail(SendEmailRequest request, ServerCallContext context)
        {
            await messageOrchestrator.Process<SendEmailNotificationCommand>(request, context);

            return new Empty();
        }

        public override async Task<Empty> SendPush(SendPushRequest request, ServerCallContext context)
        {
            await messageOrchestrator.Process<SendPushNotificationCommand>(request, context);
            return new Empty();
        }
    }
}
