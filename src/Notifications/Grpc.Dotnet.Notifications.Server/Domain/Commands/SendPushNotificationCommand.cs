using System.Collections.Generic;
using MediatR;

namespace Grpc.Dotnet.Notifications.Server.Domain.Commands
{
    public class SendPushNotificationCommand : IRequest
    {
        public string Notification { get; set; }
        public IList<string> Recipients { get; set; }
    }
}
