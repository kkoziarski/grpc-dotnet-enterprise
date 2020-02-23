
using AutoMapper;
using Grpc.Dotnet.Permissions.Server.Domain.Queries;
using Grpc.Dotnet.Permissions.V1;

namespace Grpc.Dotnet.Permissions.Server
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<IsUserAllowedRequest, IsUserAllowedQuery>();

            CreateMap<UserPermissionsRequest, UserPermissionsQuery>();
        }
    }
}
