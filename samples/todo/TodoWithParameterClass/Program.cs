using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Todos;

var app = WebApplication.Create(args);

app.Map("/api/todos",
    withParameter: http => new TodoApi(http, new TodoDbContext()),
    routes =>
    {
        routes.MapGet("/", api => api.GetTodos());
        routes.MapGet("/{id}", (http, api) => api.GetTodo(http.RouteValue<int>("id")));
        routes.MapPost("/", api => api.CreateTodo());
        routes.MapPost("/{id}", (http, api) => api.UpdateCompleted(http.RouteValue<int>("id")));
        routes.MapDelete("/{id}", (http, api) => api.DeleteTodo(http.RouteValue<int>("id")));
    });

await app.RunAsync();
