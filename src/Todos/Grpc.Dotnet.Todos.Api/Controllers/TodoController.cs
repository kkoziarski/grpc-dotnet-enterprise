using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Dotnet.Todos.Domain.Commands;
using Grpc.Dotnet.Todos.Domain.Queries;
using Grpc.Dotnet.Todos.Domain.Result;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Grpc.Dotnet.Todos
{
    [ApiController]
    [Route("/api/todos")]
    public class TodoController : ControllerBase
    {
        private readonly IMediator mediator;

        public TodoController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoResult>>> GetAll([FromHeader(Name = "user-id")]Guid? userId)
        {
            var todos = await mediator.Send(new AllTodosQuery { UserId = userId });

            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResult>> Get(long id, [FromHeader(Name = "user-id")]Guid? userId)
        {
            var todo = await mediator.Send(new TodoDetailsQuery { Id = id, UserId = userId });
            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CreateTodoCommand command, [FromHeader(Name = "user-id")]Guid userId)
        {
            command.UserId = userId;
            await mediator.Send(command);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id, [FromHeader(Name = "user-id")]Guid? userId)
        {
            await mediator.Send(new DeleteTodoCommand { Id = id, UserId = userId });
            return Ok();
        }
    }
}