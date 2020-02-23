namespace Grpc.Dotnet.Shared.Helpers.Authorization
{
    using System;

    public class UserContext : IUserContext
    {
        private readonly Guid? userId;

        public UserContext(string userName, Guid? userId, string fullName, string email)
        {
            UserName = userName;
            FullName = fullName;
            this.userId = userId;
            Email = email;
        }

        public string UserName { get; }

        public string FullName { get; }

        public string Email { get; }

        public Guid UserId
        {
            get
            {
                if (!IsAuthenticated)
                {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
                    throw new InvalidOperationException("User not authenticated. Add Authorization attribute.");
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
                }

                return userId.Value;
            }
        }

        public bool IsAuthenticated => userId.HasValue;
    }
}
