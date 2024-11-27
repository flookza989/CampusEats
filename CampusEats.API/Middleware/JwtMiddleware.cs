using CampusEats.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace CampusEats.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachUserToContext(context, userService, token);

            await _next(context);
        }

        private async Task AttachUserToContext(
            HttpContext context,
            IUserService userService,
            string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken != null)
                {
                    var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "nameid").Value);
                    context.Items["User"] = await userService.GetUserProfileAsync(userId);
                }
            }
            catch
            {
                // Token validation failed, do nothing
                // Authorization will be handled by JWT authentication middleware
            }
        }
    }

    public static class JwtMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtMiddleware>();
        }
    }
}
