using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Dotnet.Notifications.V1;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Grpc.Dotnet.Todos.Domain.Commands;
using MediatR;

namespace Grpc.Dotnet.Todos.Domain.CommandHandlers
{
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, long>
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;
        private readonly IServiceClient<NotificationService.NotificationServiceClient> notificationClient;
        private readonly IServiceClient<PermissionsService.PermissionsServiceClient> permissionsClient;

        public CreateTodoCommandHandler(
            TodoDbContext context,
            IMapper mapper,
            IServiceClient<NotificationService.NotificationServiceClient> notificationClient,
            IServiceClient<PermissionsService.PermissionsServiceClient> permissionsClient)
        {
            this.context = context;
            this.mapper = mapper;
            this.notificationClient = notificationClient;
            this.permissionsClient = permissionsClient;
        }

        public async Task<long> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var isUserAllowedReq = new IsUserAllowedRequest { Permission = "CREATE_TODO", UserId = request.UserId.ToString() };
            var isUserAllowedRes = permissionsClient.Execute<IsUserAllowedRequest, IsUserAllowedResponse>(c => c.IsUserAllowed, isUserAllowedReq);

            if (isUserAllowedRes.IsAllowed == false)
            {
                throw new InvalidOperationException("User is not allowed");
            }

            var todo = this.mapper.Map<CreateTodoCommand, Todo>(request);
            context.Todos.Add(todo);

            await context.SaveChangesAsync(cancellationToken);

            var sendEmailReq = new SendEmailRequest
            {
                Body = new SendEmailRequest.Types.Body { Text = $"Body: {request.Name}" },
                Subject = new SendEmailRequest.Types.Subject { Text = $"Subj: {request.Name} - {request.IsComplete}" },
                BodyType = "html"
            };
            sendEmailReq.Recipients.Add(new SendEmailRequest.Types.Recipient
            {
                Email = $"{request.UserId}@example.com",
                Name = request.UserId.ToString()
            });

            notificationClient.Execute(c => c.SendEmail, sendEmailReq);

            return todo.Id;
        }
    }
}
