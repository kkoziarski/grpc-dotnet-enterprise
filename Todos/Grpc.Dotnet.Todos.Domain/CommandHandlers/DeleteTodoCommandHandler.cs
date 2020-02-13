using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Dotnet.Notifications.V1;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.Exceptions;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Grpc.Dotnet.Todos.Domain.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Grpc.Dotnet.Todos.Domain.CommandHandlers
{
    public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Unit>
    {
        private readonly TodoDbContext context;
        private readonly IServiceClient<NotificationService.NotificationServiceClient> notificationClient;
        private readonly IServiceClient<PermissionsService.PermissionsServiceClient> permissionsClient;

        public DeleteTodoCommandHandler(
            TodoDbContext context,
            IServiceClient<NotificationService.NotificationServiceClient> notificationClient,
            IServiceClient<PermissionsService.PermissionsServiceClient> permissionsClient)
        {
            this.context = context;
            this.notificationClient = notificationClient;
            this.permissionsClient = permissionsClient;
        }

        public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var isUserAllowedReq = new IsUserAllowedRequest { Permission = "DELETE_TODO", UserId = request.UserId.ToString() };
            var isUserAllowedRes = permissionsClient.Execute<IsUserAllowedRequest, IsUserAllowedResponse>(c => c.IsUserAllowed, isUserAllowedReq);

            if (isUserAllowedRes.IsAllowed == false)
            {
                throw new InvalidOperationException("User is not allowed");
            }

            var todo = await context.Todos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (todo == null)
            {
                throw new EntityNotFoundException<Todo>(request.Id);
            }

            context.Todos.Remove(todo);
            await context.SaveChangesAsync(cancellationToken);

            var sendEmailReq = new SendPushRequest
            {
                Message = new SendPushRequest.Types.Message { Text = $"Todo {request.Id} has been deleted"}
            };
            sendEmailReq.Recipients.Add(new SendPushRequest.Types.Recipient
            {
                Name = request.UserId.ToString()
            });

            notificationClient.Execute(c => c.SendPush, sendEmailReq);

            return Unit.Value;
        }
    }
}
