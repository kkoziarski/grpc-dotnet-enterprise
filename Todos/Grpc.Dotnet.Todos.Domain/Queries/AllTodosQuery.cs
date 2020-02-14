using System;
using System.Collections.Generic;
using Grpc.Dotnet.Todos.Domain.Result;
using MediatR;

namespace Grpc.Dotnet.Todos.Domain.Queries
{
    public class AllTodosQuery : IRequest<List<TodoResult>>
    {
        public Guid? UserId { get; set; }

    }
}