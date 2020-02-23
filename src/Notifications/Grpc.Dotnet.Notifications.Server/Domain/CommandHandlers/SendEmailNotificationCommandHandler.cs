using System.Threading;
using System.Threading.Tasks;
using Grpc.Dotnet.Notifications.Server.Domain.Commands;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Grpc.Dotnet.Notifications.Server.Domain.CommandHandlers
{
    public class SendEmailNotificationCommandHandler : IRequestHandler<SendEmailNotificationCommand, Unit>
    {
        private readonly ILogger<SendEmailNotificationCommandHandler> logger;

        public SendEmailNotificationCommandHandler(ILogger<SendEmailNotificationCommandHandler> logger)
        {
            this.logger = logger;
        }

        public Task<Unit> Handle(SendEmailNotificationCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Email sent. Subject: {request.Subject}");
            return Task.FromResult(Unit.Value);
        }
    }
}
