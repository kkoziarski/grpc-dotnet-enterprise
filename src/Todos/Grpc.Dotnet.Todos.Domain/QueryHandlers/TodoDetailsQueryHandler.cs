using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.Rpc.Client;
using Grpc.Dotnet.Todos.Domain.Queries;
using Grpc.Dotnet.Todos.Domain.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Grpc.Dotnet.Todos.Domain.QueryHandlers
{
    public class TodoDetailsQueryHandler : IRequestHandler<TodoDetailsQuery, TodoResult>
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;
        private readonly IServiceClient<PermissionsService.PermissionsServiceClient> permissionsClient;

        public TodoDetailsQueryHandler(
            TodoDbContext context,
            IMapper mapper,
            IServiceClient<PermissionsService.PermissionsServiceClient> permissionsClient)
        {
            this.context = context;
            this.mapper = mapper;
            this.permissionsClient = permissionsClient;
        }

        public async Task<TodoResult> Handle(TodoDetailsQuery request, CancellationToken cancellationToken)
        {
            var isUserAllowedReq = new IsUserAllowedRequest { Permission = "READ_TODO", UserId = request.UserId?.ToString() ?? string.Empty };
            var isUserAllowedRes = permissionsClient.Execute<IsUserAllowedRequest, IsUserAllowedResponse>(c => c.IsUserAllowed, isUserAllowedReq);

            if (isUserAllowedRes.IsAllowed == false)
            {
                throw new InvalidOperationException("User is not allowed");
            }

            var todo = await context
                .Todos
                .ProjectTo<TodoResult>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

            ////if (todo == null)
            ////{
            ////    throw new EntityNotFoundException<Todo>(request.Id);
            ////}

            return todo;
        }
    }
}