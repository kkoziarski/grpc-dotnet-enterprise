using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Dotnet.Todos.Domain.Commands;
using Grpc.Dotnet.Todos.Domain.Queries;
using Grpc.Dotnet.Todos.Domain.Result;
using MediatR;
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
        public async Task<ActionResult<List<TodoResult>>> GetAll()
        {

            var todos = await mediator.Send(new AllTodosQuery());

            return todos;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoResult>> Get(long id)
        {
            var todo = await mediator.Send(new TodoDetailsQuery { Id = id });
            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        [HttpPost]
        public async Task Post(CreateTodoCommand command)
        {
            await mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await mediator.Send(new DeleteTodoCommand { Id = id });
            return Ok();
        }
    }
}