using System;
using Grpc.Dotnet.Shared.Helpers.Authorization;
using Microsoft.Extensions.Configuration;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class UserContextMock : UserContext
    {
        public const string AuthenticationSchemeType = "Test";
        private const string Identity = "ed4ce7fd-9794-45df-b482-192814db4336";
        private const string IntegrationUserName = "IntegrationTestUser";
        private const string IntegrationFullName = "Integration Test User";
        private const string IntegrationEmail = "integration@test.user";

        public UserContextMock(IConfiguration configuration) 
            : base(
                configuration.GetValue<string>("User:userName", IntegrationUserName), 
                configuration.GetValue<Guid>("User:userId", Guid.Parse(Identity)), 
                configuration.GetValue<string>("User:fullName", IntegrationFullName), 
                configuration.GetValue<string>("User:email", IntegrationEmail))

        {
        }
    }
}