namespace Grpc.Dotnet.Shared.Helpers.Startup
{
    using System;
    using System.Linq;
    using Grpc.Dotnet.Shared.Helpers.Authorization;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;

    public static class StartupUserContext
    {
        private const string UserNameClaimType = "name";
        private const string UserIdClaimType = "id";
        private const string SubClaimType = "sub";
        private const string UserFullNameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        private const string EmailClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

        public static IServiceCollection AddConfiguredUserContext(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUserContext, UserContext>(uc =>
            {
                var claims = uc.GetService<IActionContextAccessor>().ActionContext.HttpContext.User.Claims.ToList();

                if (claims.Any())
                {
                    var userId = claims.SingleOrDefault(x => x.Type == UserIdClaimType)?.Value ?? claims.SingleOrDefault(x => x.Type == SubClaimType)?.Value;
                    var userName = claims.SingleOrDefault(x => x.Type == UserNameClaimType)?.Value;
                    var fullName = claims.SingleOrDefault(x => x.Type == UserFullNameClaimType)?.Value;
                    var email = claims.SingleOrDefault(x => x.Type == EmailClaimType)?.Value;
                    return new UserContext(userName, new Guid(userId ?? Guid.Empty.ToString()), fullName, email);
                }

                return new UserContext(null, null, null, null);
            });
            return services;
        }
    }
}
