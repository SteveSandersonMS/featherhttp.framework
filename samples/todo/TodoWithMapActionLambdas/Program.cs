using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>();

var app = builder.Build();
app.UseDeveloperExceptionPage();

app.MapAction("get", "/api/todos", async (HttpContext http, TodoDbContext db) =>
{
    var todos = await db.Todos.ToListAsync();
    await http.Response.WriteAsJsonAsync(todos);
});

app.MapAction("post", "/api/todos", async (HttpContext http, TodoDbContext db, Todo newTodo) =>
{
    await db.Todos.AddAsync(newTodo);
    await db.SaveChangesAsync();

    http.Response.StatusCode = 204;
});

app.MapAction("post", "/api/todos/{id}", async (HttpContext http, TodoDbContext db, int id, Todo updatedTodo) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
    {
        http.Response.StatusCode = 404;
        return;
    }

    todo.IsComplete = updatedTodo.IsComplete;
    await db.SaveChangesAsync();
    http.Response.StatusCode = 204;
});

app.MapAction("delete", "/api/todos/{id}", async (HttpContext http, TodoDbContext db, int id) =>
{
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
