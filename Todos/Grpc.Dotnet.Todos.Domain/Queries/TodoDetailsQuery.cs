using Grpc.Dotnet.Todos.Domain.Result;
using MediatR;

namespace Grpc.Dotnet.Todos.Domain.Queries
{
    public class TodoDetailsQuery : IRequest<TodoResult>
    {
        public long Id { get; set; }
    }
}