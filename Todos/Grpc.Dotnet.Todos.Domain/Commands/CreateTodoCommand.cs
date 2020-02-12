using MediatR;

namespace Grpc.Dotnet.Todos.Domain.Commands
{
    public class CreateTodoCommand : IRequest<long>
    {
        public string Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
