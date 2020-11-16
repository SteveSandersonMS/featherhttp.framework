using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Todos;

var app = WebApplication.Create(args);

app.MapGet("/api/todos", async http =>
{
    using var db = new TodoDbContext();
    var todos = await db.Todos.ToListAsync();
    await http.Response.WriteAsJsonAsync(todos);
});

app.MapGet("/api/todos/{id:int}", async http =>
{
    using var db = new TodoDbContext();
    var id = http.Request.RouteValues.Get<int>("id").Value;
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    await http.Response.WriteAsJsonAsync(todo);
});

app.MapPost("/api/todos", async http =>
{
    var todo = await http.Request.ReadFromJsonAsync<Todo>();

    using var db = new TodoDbContext();
    await db.Todos.AddAsync(todo);
    await db.SaveChangesAsync();

    http.Response.StatusCode = 204;
});

app.MapPost("/api/todos/{id:int}", async http =>
{
    using var db = new TodoDbContext();
    var id = http.Request.RouteValues.Get<int>("id").Value;
    var todo = await db.Todos.FindAsync(id);

    if (todo == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    var inputTodo = await http.Request.ReadFromJsonAsync<Todo>();
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    http.Response.StatusCode = 204;
});

app.MapDelete("/api/todos/{id}", async http =>
{
    using var db = new TodoDbContext();
    var id = http.Request.RouteValues.Get<int>("id").Value;
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    http.Response.StatusCode = 204;
});

await app.RunAsync();
