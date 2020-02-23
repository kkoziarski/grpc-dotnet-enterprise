using System;
using MediatR;

namespace Grpc.Dotnet.Permissions.Server.Domain.Queries
{
    public class IsUserAllowedQuery : IRequest<bool>
    {
        public Guid UserId { get; set; }

        public string Permission { get; set; }
    }
}
