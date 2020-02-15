using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Dotnet.Notifications.V1;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.Exceptions;
using Grpc.Dotnet.Shared.Helpers.UnitTests.Helpers;
using Grpc.Dotnet.Todos.Domain.CommandHandlers;
using Grpc.Dotnet.Todos.Domain.Commands;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using Xunit;

namespace Grpc.Dotnet.Todos.Domain.UnitTests
{
    public class DeleteTodoCommandHandlerTests
    {
        private RpcClientMock<PermissionsService.PermissionsServiceClient> rpcPermissionsMock;
        private RpcClientMock<NotificationService.NotificationServiceClient> rpcNotificationsMock;

        public DeleteTodoCommandHandlerTests()
        {
            this.rpcPermissionsMock = new RpcClientMock<PermissionsService.PermissionsServiceClient>();
            this.rpcNotificationsMock = new RpcClientMock<NotificationService.NotificationServiceClient>();

            var rpcResponse = new IsUserAllowedResponse { IsAllowed = true };
            this.rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotHavePermission_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteTodoCommand { UserId = userId, Id = -1 };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = false };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                // Act
                var sut = new DeleteTodoCommandHandler(context, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
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
            var command = new DeleteTodoCommand { UserId = userId, Id = 1 };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = true };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var context = new TodoDbContext(dbOptions))
            {
                context.Todos.Add(new Todo { Id = 1, Name = "todo 1" });
                context.SaveChanges();

                var sut = new DeleteTodoCommandHandler(context, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
                // Act
                var result = await sut.Handle(command, new CancellationToken(false));
            }
        }

        [Fact]
        public async Task Handle_WhenTodoDoesNotExist_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteTodoCommand { UserId = userId, Id = -1 };

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                var sut = new DeleteTodoCommandHandler(context, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
                try
                {
                    // Act
                    var result = await sut.Handle(command, new CancellationToken(false));
                    Assert.True(false, "Should throw exception");
                }
                catch (EntityNotFoundException<Todo> ex)
                {
                    // Assert
                    Assert.NotNull(ex);
                }
            }
        }

        [Fact]
        public async Task Handle_WhenUserHasPermission_ShouldDeleteTodo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteTodoCommand { UserId = userId, Id = 2 };

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            using (var context = new TodoDbContext(dbOptions))
            {
                context.Todos.Add(new Todo { Id = 1, Name = "todo 1" });
                context.Todos.Add(new Todo { Id = 2, Name = "todo 2" });
                context.SaveChanges();

                var sut = new DeleteTodoCommandHandler(context, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);
                // Act
                await sut.Handle(command, new CancellationToken(false));
                var detetedTodo = await context.Todos.FirstOrDefaultAsync(t => t.Id == command.Id);

                // Assert
                detetedTodo.ShouldBeNull();
            }
        }

        [Fact]
        public async Task Handle_WhenTodoDeleted_ShouldNotifiyUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteTodoCommand { UserId = userId, Id = 1 };

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                context.Todos.Add(new Todo { Id = 1, Name = "todo 1" });
                context.SaveChanges();

                var sut = new DeleteTodoCommandHandler(context, rpcNotificationsMock.ServiceClient, rpcPermissionsMock.ServiceClient);

                // Act
                await sut.Handle(command, new CancellationToken(false));
            }

            // Assert
            rpcNotificationsMock.ClientMock.Verify(x => x.SendPush(It.IsAny<SendPushRequest>(), It.IsAny<CallOptions>()), Times.Once);
        }
    }
}