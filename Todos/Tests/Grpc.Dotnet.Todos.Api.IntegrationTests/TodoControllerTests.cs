using System.Net;
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
        public async Task GetAll_WhenCall_ShouldReturn200Code()
        {
            //Arrange
            using var client = this.CreateClient();

            var rpcPermissionsClientMock = GetRpcClientMock<PermissionsService.PermissionsServiceClient>();
            rpcPermissionsClientMock.SetMock(this.rpcPermissionsMock.Object);

            //Act
            var response = await client.GetAsync("api/todos");
            ////rpcPermissionsClientMock.ClearMock();

            //Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAll_WhenCall_ShouldCreateRPCRequest()
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
    }
}
