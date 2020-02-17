using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Dotnet.Notifications.V1;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.IntegrationTests;
using Grpc.Dotnet.Todos.Domain;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Shouldly;
using Grpc.Dotnet.Todos.Domain.Result;
using System.Net.Http;
using Grpc.Dotnet.Todos.Domain.Commands;
using System;
using System.Text;
using System.Linq;

namespace Grpc.Dotnet.Todos.Api.IntegrationTests
{
    public class TodoControllerTests : ControllerTestBase<Startup, TodoDbContext>
    {
        private readonly ITestOutputHelper output;
        private readonly Mock<PermissionsService.PermissionsServiceClient> rpcPermissionsMock;

        public TodoControllerTests(
            WebApplicationFactoryBase<Startup, TodoDbContext> factory,
            ITestOutputHelper output)
            : base(factory)
        {
            this.output = output;

            this.rpcPermissionsMock = new Mock<PermissionsService.PermissionsServiceClient>();
            this.rpcPermissionsMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>()))
                .Returns(new IsUserAllowedResponse { IsAllowed = true });

        }

        [Fact]
        public async Task GetAll_WhenCalled_ShouldReturn200Code()
        {
            //Arrange
            using var client = this.CreateClient();

            var rpcPermissionsClientMock = GetRpcClientMock<PermissionsService.PermissionsServiceClient>();
            rpcPermissionsClientMock.SetMock(this.rpcPermissionsMock.Object);

            //Act
            var response = await client.GetAsync("api/todos");

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_WhenCalled_ShouldCreateRPCRequest()
        {
            using var client = this.CreateClient();

            // Arrange
            var rpcPermissionsClientMock = GetRpcClientMock<PermissionsService.PermissionsServiceClient>();
            rpcPermissionsClientMock.SetMock(this.rpcPermissionsMock.Object);

            // Act
            var response = await client.GetAsync("api/todos");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            GetRpcClientMock<PermissionsService.PermissionsServiceClient>()
                .EnsureRequestCount<IsUserAllowedRequest>(1)
                .EnsureAllRequestsCount(1);

            GetRpcClientMock<NotificationService.NotificationServiceClient>()
                .EnsureAllRequestsCount(0);
        }

        [Fact]
        public async Task Get_WhenCalled_ShouldReturn200Code()
        {
            //Arrange
            using var client = this.CreateClient();

            this.DbContext.Todos.AddRange(new Todo[]
            {
                new Todo { Name = "todo 1" },
                new Todo { Name = "todo 4 get" }
            });
            this.DbContext.SaveChanges();

            var todo = this.DbContext.Todos.FirstOrDefault(t => t.Name == "todo 4 get");

            var rpcPermissionsClientMock = GetRpcClientMock<PermissionsService.PermissionsServiceClient>();
            rpcPermissionsClientMock.SetMock(this.rpcPermissionsMock.Object);

            //Act
            var response = await client.GetAsync($"api/todos/{todo.Id}");

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var resultString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<TodoResult>(resultString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            result.Id.ShouldBe(todo.Id);
            result.Name.ShouldBe(todo.Name);
        }

        [Fact]
        public async Task Post_WhenCalled_ShouldReturn201Code()
        {
            //Arrange
            using var client = this.CreateClient();
            var userId = Guid.NewGuid();

            client.DefaultRequestHeaders.Add("user-id", userId.ToString());

            var rpcPermissionsClientMock = GetRpcClientMock<PermissionsService.PermissionsServiceClient>();
            rpcPermissionsClientMock.SetMock(this.rpcPermissionsMock.Object);

            var commandRequest = new CreateTodoCommand { Name = "todo integration", UserId = userId, IsComplete = true };

            //Act
            var response = await client.PostAsync($"api/todos", new StringContent(JsonSerializer.Serialize(commandRequest), Encoding.UTF8, "application/json"));

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Delete_WhenCalled_ShouldReturn200Code()
        {
            //Arrange
            using var client = this.CreateClient();
            var userId = Guid.NewGuid();

            this.DbContext.Todos.AddRange(new Todo[]
            {
                new Todo { Name = "todo 1" },
                new Todo { Name = "todo 4 delete" }
            });
            this.DbContext.SaveChanges();

            var todo = this.DbContext.Todos.FirstOrDefault(t => t.Name == "todo 4 delete");

            client.DefaultRequestHeaders.Add("user-id", userId.ToString());

            var rpcPermissionsClientMock = GetRpcClientMock<PermissionsService.PermissionsServiceClient>();
            rpcPermissionsClientMock.SetMock(this.rpcPermissionsMock.Object);

            //Act
            var response = await client.DeleteAsync($"api/todos/{todo.Id}");

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }
    }
}
