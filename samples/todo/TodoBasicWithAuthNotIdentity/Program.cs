using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Todos;

var app = WebApplication.Create(args);

new TodoApi().MapRoutes(app);

await app.RunAsync();
