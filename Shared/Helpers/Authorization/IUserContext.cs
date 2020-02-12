namespace Grpc.Dotnet.Shared.Helpers.Authorization
{
    using System;

    public interface IUserContext
    {
        string UserName { get; }

        Guid UserId { get; }

        string FullName { get; }

        bool IsAuthenticated { get; }

        string Email { get; }
    }
}
