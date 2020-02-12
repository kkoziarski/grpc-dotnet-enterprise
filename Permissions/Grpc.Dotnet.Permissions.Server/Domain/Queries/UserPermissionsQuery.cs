using System;
using System.Collections.Generic;
using MediatR;

namespace Grpc.Dotnet.Permissions.Server.Domain.Queries
{
    public class UserPermissionsQuery : IRequest<IEnumerable<string>>
    {
        public Guid UserId { get; set; }
    }
}
