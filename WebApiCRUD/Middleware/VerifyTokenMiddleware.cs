using System.IdentityModel.Tokens.Jwt;

namespace WebApiCRUD.Middleware
{
    public class VerifyTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public VerifyTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(' ')[1];

                if (token == null)
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return;
                }
                var expirationDate = jwtToken.ValidTo;

                if (expirationDate < DateTime.UtcNow)
                {
                    context.Response.Cookies.Delete("jwt");
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    return;
                }
                //await _next(context);
            }

            await _next(context);
        }
    }
}
