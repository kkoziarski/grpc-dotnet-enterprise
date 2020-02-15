using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Grpc.Dotnet.Permissions.V1;
using Grpc.Dotnet.Shared.Helpers.UnitTests.Helpers;
using Grpc.Dotnet.Todos.Domain.Domain;
using Grpc.Dotnet.Todos.Domain.Queries;
using Grpc.Dotnet.Todos.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using Xunit;

namespace Grpc.Dotnet.Todos.Domain.UnitTests
{
    public class AllTodosQueryHandlerTests
    {
        private RpcClientMock<PermissionsService.PermissionsServiceClient> rpcPermissionsMock;
        private IMapper mapper;

        public AllTodosQueryHandlerTests()
        {
            this.rpcPermissionsMock = new RpcClientMock<PermissionsService.PermissionsServiceClient>();
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
            this.mapper = new Mapper(configuration);
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotHavePermission_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new AllTodosQuery { UserId = userId };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = false };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                // Act
                var sut = new AllTodosQueryHandler(context, mapper, rpcPermissionsMock.ServiceClient);
                try
                {
                    var result = await sut.Handle(request, new CancellationToken(false));
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
            var request = new AllTodosQuery { UserId = userId };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = true };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                var sut = new AllTodosQueryHandler(context, mapper, rpcPermissionsMock.ServiceClient);
                // Act
                var result = await sut.Handle(request, new CancellationToken(false));
            }
        }

        [Fact]
        public async Task Handle_WhenUserHasPermission_ShouldReturnAllTodos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new AllTodosQuery { UserId = userId };
            var rpcResponse = new IsUserAllowedResponse { IsAllowed = true };
            rpcPermissionsMock.ClientMock.Setup(x => x.IsUserAllowed(It.IsAny<IsUserAllowedRequest>(), It.IsAny<CallOptions>())).Returns(rpcResponse);

            var dbOptions = new DbContextOptionsBuilder<TodoDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            using (var context = new TodoDbContext(dbOptions))
            {
                context.Todos.Add(new Todo { Id = 1, Name = "todo 1" });
                context.Todos.Add(new Todo { Id = 2, Name = "todo 2" });
                context.SaveChanges();

                var sut = new AllTodosQueryHandler(context, mapper, rpcPermissionsMock.ServiceClient);
                // Act
                var result = await sut.Handle(request, new CancellationToken(false));

                result.Count.ShouldBe(2);
                result[0].Id.ShouldBe(1);
                result[0].Name.ShouldBe("todo 1");
                result[1].Id.ShouldBe(2);
                result[1].Name.ShouldBe("todo 2");
            }
        }
    }
}
