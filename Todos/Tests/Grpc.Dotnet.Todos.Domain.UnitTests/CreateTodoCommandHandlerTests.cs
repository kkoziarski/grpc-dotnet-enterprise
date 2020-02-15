using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Grpc.Dotnet.Notifications.V1;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.UnitTests.Helpers;
using Grpc.Dotnet.Todos.Domain.CommandHandlers;
using Grpc.Dotnet.Todos.Domain.Commands;
using Grpc.Dotnet.Todos.Domain.Domain;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using Xunit;

namespace Grpc.Dotnet.Todos.Domain.UnitTests
{
    public class CreateTodoCommandHandlerTests
    {
        private RpcClientMock<PermissionsService.PermissionsServiceClient> rpcPermissionsMock;
        private RpcClientMock<NotificationService.NotificationServiceClient> rpcNotificationsMock;
        private IMapper mapper;

        public CreateTodoCommandHandlerTests()
        {

            this.rpcPermissionsMock = new RpcClientMock<PermissionsService.PermissionsServiceClient>();
            this.rpcNotificationsMock = new RpcClientMock<NotificationService.NotificationServiceClient>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            this.mapper = new Mapper(configuration);

            var rpcResponse = new IsUserAllowedResponse { IsAllowed = true };
            this.rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotHavePermission_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTodoCommand { UserId = userId, Name = "todo one", IsComplete = true };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = false };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                // Act
                var sut = new CreateTodoCommandHandler(context, mapper, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
                try
                {
                    var result = await sut.Handle(command, new CancellationToken(false));
                    Assert.True(false, "Should throw exception");
                }
                catch (InvalidOperationException ex)
                {
                    // Assert
                    Assert.NotNull(ex);
                }
            }
        }

        [Fact]
        public async Task Handle_WhenUserHasPermission_ShouldNotThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTodoCommand { UserId = userId, Name = "todo one", IsComplete = true };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = true };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                var sut = new CreateTodoCommandHandler(context, mapper, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
                // Act
                var result = await sut.Handle(command, new CancellationToken(false));
            }
        }

        [Fact]
        public async Task Handle_WhenUserHasPermission_ShouldCreateTodo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTodoCommand { UserId = userId, Name = "todo one", IsComplete = true };

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            long todoId = 0;
            using (var context = new TodoDbContext(dbOptions))
            {
                var sut = new CreateTodoCommandHandler(context, mapper, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
                // Act
                todoId = await sut.Handle(command, new CancellationToken(false));

                // Assert
                todoId.ShouldBeGreaterThan(0);
            }

            using (var context = new TodoDbContext(dbOptions))
            {
                var createdTodo = await context.Todos.FirstOrDefaultAsync(t => t.Id == todoId);

                // Assert
                createdTodo.ShouldNotBeNull();
                createdTodo.Name.ShouldBe(command.Name);
                createdTodo.IsComplete.ShouldBe(command.IsComplete);
            }
        }

        [Fact]
        public async Task Handle_WhenTodoCreated_ShouldNotifiyUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTodoCommand { UserId = userId, Name = "todo one", IsComplete = true };

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var context = new TodoDbContext(dbOptions))
            {
                var sut = new CreateTodoCommandHandler(context, mapper, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);

                // Act
                await sut.Handle(command, new CancellationToken(false));
            }

            // Assert
            rpcNotificationsMock.ClientMock.Verify(x => x.SendEmail(It.IsAny<SendEmailRequest>(), It.IsAny<CallOptions>()), Times.Once);
        }
    }
}
