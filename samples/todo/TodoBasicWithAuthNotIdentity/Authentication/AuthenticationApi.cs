using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Authentication
{
    public class AuthenticationApi
    {
        private readonly JwtAuth _jwtAuth;

        public AuthenticationApi(JwtAuth jwtAuth)
        {
            _jwtAuth = jwtAuth;
        }

        public void MapRoutes(WebApplication app)
        {
            app.MapGet("/api/auth/login", async http =>
            {
                await http.Response.WriteAsJsonAsync(new
                {
                    token = _jwtAuth.GenerateToken("Some Username")
                });
            });
        }
    }
}
