using System;
using System.Linq;
using Grpc.Dotnet.Permissions.Domain;
using Grpc.Dotnet.Permissions.V1;
using Shouldly;
using Xunit;

namespace Grpc.Dotnet.Permissions.Server.IntegrationTests
{
    public class PermissionsServiceTest : PermissionsServerTestBase<PermissionsService.PermissionsServiceClient>
    {
        public PermissionsServiceTest(PermissionsServerApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public void IsUserAllowed_ForCreateTodoPermissions_ShouldReturnFalse()
        {
            // Arrange
            var isUserAllowedReq = new IsUserAllowedRequest { Permission = "CREATE_TODO", UserId = Guid.NewGuid().ToString() };

            // Act
            var response = this.Client.IsUserAllowed(isUserAllowedReq);

            // Assert
            Assert.False(response.IsAllowed);
        }

        [Fact]
        public void GetUserPermissions_RandomUser_ShouldReturnReadPermissions()
        {
            // Arrange
            var userPermissionsReq = new UserPermissionsRequest { UserId = Guid.NewGuid().ToString() };

            // Act
            var response = this.Client.GetUserPermissions(userPermissionsReq);

            // Assert
            Assert.NotNull(response.Permissions);
            response.Permissions.AsEnumerable().ShouldContain("READ");
            response.Permissions.AsEnumerable().ShouldNotContain("DELETE");
        }

        [Fact]
        public void GetUserPermissions_AdminUser_ShouldReturnDeletePermission()
        {
            // Arrange
            var userPermissionsReq = new UserPermissionsRequest { UserId = DomainModule.AdminId.ToString() };

            // Act
            var response = this.Client.GetUserPermissions(userPermissionsReq);

            // Assert
            Assert.NotNull(response.Permissions);
            response.Permissions.AsEnumerable().ShouldContain("DELETE");
        }
    }
}
