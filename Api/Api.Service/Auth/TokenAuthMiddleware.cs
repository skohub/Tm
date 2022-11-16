using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Api.Service.Services;
using Serilog.Context;

namespace Api.Service.Auth
{
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IUserService _userService;

        public TokenAuthMiddleware(RequestDelegate next, IUserService userService)
        {
            _next = next;
            _userService = userService;
        }

        public async Task Invoke(HttpContext context)
        {
            var headers = context.Request.Headers["Authorization"];
            if (!headers.Any())
            {
                context.Response.StatusCode = 401;
                return;
            }

            var authenticationHeader = AuthenticationHeaderValue.Parse(headers);
            var token = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationHeader.Parameter!));
            var user = _userService.GetUserByToken(token);
            if (user == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Name) };
            if (user.ConnectionStringName != null)
            {
                claims.Add(new Claim(TokenClaimTypes.ConnectionStringName, user.ConnectionStringName));
            }
            
            var identity = new ClaimsIdentity(claims);
            context.User = new ClaimsPrincipal(identity);

            using (LogContext.PushProperty("Username", user.Name))
            {
                await _next(context);
            }
        }
    }
}
