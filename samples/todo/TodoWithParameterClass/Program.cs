using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Todos;

var app = WebApplication.Create(args);

app.Map("/api/todos",
    withParameter: http => new TodoApi(http, new TodoDbContext()),
    routes =>
    {
        routes.MapGet("/", api => api.GetTodos());
        routes.MapGet("/{id}", api => api.GetTodo());
        routes.MapPost("/", api => api.CreateTodo());
        routes.MapPost("/{id}", api => api.UpdateCompleted());
        routes.MapDelete("/{id}", api => api.DeleteTodo());
    });

await app.RunAsync();
