namespace Grpc.Dotnet.Shared.Helpers.Authorization
{
    public interface IAuthorizeService
    {
        bool IsCurrentUserAuthorized(string permission);
    }
}