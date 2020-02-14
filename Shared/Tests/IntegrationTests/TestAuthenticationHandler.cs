using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Grpc.Dotnet.Shared.Helpers.IntegrationTests
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly UserContextMock userContext;

        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            this.userContext = new UserContextMock(configuration);
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, this.userContext.UserName) };
            var identity = new ClaimsIdentity(claims, UserContextMock.AuthenticationSchemeType);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, UserContextMock.AuthenticationSchemeType);

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }
    }
}