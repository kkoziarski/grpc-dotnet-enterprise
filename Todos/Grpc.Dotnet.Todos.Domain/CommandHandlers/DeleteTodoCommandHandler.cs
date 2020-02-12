using System.Threading;
using System.Threading.Tasks;
using Grpc.Dotnet.Shared.Helpers.Exceptions;
using Grpc.Dotnet.Todos.Domain.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Grpc.Dotnet.Todos.Domain.CommandHandlers
{
    public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Unit>
    {
        private readonly TodoDbContext context;

        public DeleteTodoCommandHandler(TodoDbContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await context.Todos.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (todo == null)
            {
                throw new EntityNotFoundException<Todo>(request.Id);
            }

            context.Todos.Remove(todo);
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
