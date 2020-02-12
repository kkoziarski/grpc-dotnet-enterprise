using MediatR;

namespace Grpc.Dotnet.Todos.Domain.Commands
{
    public class DeleteTodoCommand : IRequest
    {
        public long Id { get; set; }
    }
}
