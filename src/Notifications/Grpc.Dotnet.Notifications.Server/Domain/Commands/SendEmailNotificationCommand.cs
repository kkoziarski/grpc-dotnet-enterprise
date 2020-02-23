using System.Collections.Generic;
using MediatR;

namespace Grpc.Dotnet.Notifications.Server.Domain.Commands
{
    public class SendEmailNotificationCommand : IRequest
    {
        public string BodyType { get; set; }
        public string Subject { get; set; }

        public string Body { get; set; }

        public IList<string> Recipients { get; set; }
    }
}
