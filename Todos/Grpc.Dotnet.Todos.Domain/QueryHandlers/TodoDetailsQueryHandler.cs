using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

        public TodoDetailsQueryHandler(TodoDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<TodoResult> Handle(TodoDetailsQuery request, CancellationToken cancellationToken)
        {
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