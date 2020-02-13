using System.Threading;
using System.Threading.Tasks;
using Grpc.Dotnet.Notifications.Server.Domain.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Grpc.Dotnet.Notifications.Server.Domain.CommandHandlers
{

    public class SendPushNotificationCommandHandler : IRequestHandler<SendPushNotificationCommand, Unit>
    {
        private readonly ILogger<SendPushNotificationCommandHandler> logger;

        public SendPushNotificationCommandHandler(ILogger<SendPushNotificationCommandHandler> logger)
        {
            this.logger = logger;
        }

        public Task<Unit> Handle(SendPushNotificationCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Notification sent: {request.Notification}");
            return Task.FromResult(Unit.Value);
        }
    }

}
