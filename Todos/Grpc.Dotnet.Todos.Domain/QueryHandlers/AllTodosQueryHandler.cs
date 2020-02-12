using System.Collections.Generic;
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
    public class AllTodosQueryHandler : IRequestHandler<AllTodosQuery, List<TodoResult>>
    {
        private readonly TodoDbContext context;
        private readonly IMapper mapper;

        public AllTodosQueryHandler(TodoDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<List<TodoResult>> Handle(AllTodosQuery request, CancellationToken cancellationToken)
        {
            var todos = await context
                .Todos
                .ProjectTo<TodoResult>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return todos;
        }
    }
}