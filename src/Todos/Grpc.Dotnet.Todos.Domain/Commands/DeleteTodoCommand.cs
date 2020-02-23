using System;
using MediatR;

namespace Grpc.Dotnet.Todos.Domain.Commands
{
    public class DeleteTodoCommand : IRequest
    {
        public Guid? UserId { get; set; }

        public long Id { get; set; }
    }
}
