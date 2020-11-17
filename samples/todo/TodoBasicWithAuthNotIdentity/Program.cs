using Microsoft.AspNetCore.Builder;
using Todos;
using Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder();

var jwtAuth = new JwtAuth(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = jwtAuth.ValidationParameters;
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

new TodoApi().MapRoutes(app);
new AuthenticationApi(jwtAuth).MapRoutes(app);

await app.RunAsync();
