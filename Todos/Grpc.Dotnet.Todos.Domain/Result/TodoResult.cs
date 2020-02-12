namespace Grpc.Dotnet.Todos.Domain.Result
{
    public class TodoResult
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string IsComplete { get; set; }
    }
}
