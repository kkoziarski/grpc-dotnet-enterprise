using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Dotnet.Todos.Domain.Commands;
using MediatR;

namespace Grpc.Dotnet.Todos.Domain.CommandHandlers
{
    public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, long>
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;

        public CreateTodoCommandHandler(TodoDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<long> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {

            var todo = this.mapper.Map<CreateTodoCommand, Todo>(request);
            context.Todos.Add(todo);

            await context.SaveChangesAsync(cancellationToken);

            return todo.Id;
        }
    }
}
