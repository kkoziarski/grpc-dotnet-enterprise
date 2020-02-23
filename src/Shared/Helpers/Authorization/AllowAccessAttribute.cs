namespace Grpc.Dotnet.Shared.Helpers.Authorization
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class AllowAccessAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public AllowAccessAttribute(string permission)
        {
            Permission = permission;
        }

        public string Permission { get; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                // this path will be for UserManagement controllers only, for other microservices user will be already authenticated.
                var authenticateResult = await context.HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                if (!authenticateResult.Succeeded)
                {
                    return;
                }

                context.HttpContext.User = authenticateResult.Principal;
            }

            var authorizeService = context.HttpContext.RequestServices.GetService<IAuthorizeService>();
            var isAuthorized = authorizeService.IsCurrentUserAuthorized(Permission);

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
