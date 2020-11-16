using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var app = WebApplication.Create(args);

            app.Map("/api/todos",
                withParameter: () => new TodoDbContext(),
                routes =>
                {
                    routes.MapGet("/", GetTodos);
                    routes.MapGet("/{id}", GetTodo);
                    routes.MapPost("/", CreateTodo);
                    routes.MapPost("/{id}", UpdateCompleted);
                    routes.MapDelete("/{id}", DeleteTodo);
                });

            await app.RunAsync();
        }

        static async Task GetTodos(HttpContext http, TodoDbContext db)
        {
            var todos = await db.Todos.ToListAsync();
            await http.Response.WriteAsJsonAsync(todos);
        }

        static async Task GetTodo(HttpContext context, TodoDbContext db)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            await context.Response.WriteAsJsonAsync(todo);
        }

        static async Task CreateTodo(HttpContext context, TodoDbContext db)
        {
            var todo = await context.Request.ReadFromJsonAsync<Todo>();

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        static async Task UpdateCompleted(HttpContext context, TodoDbContext db)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await db.Todos.FindAsync(id);

            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var inputTodo = await context.Request.ReadFromJsonAsync<Todo>();
            todo.IsComplete = inputTodo.IsComplete;

            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }

        static async Task DeleteTodo(HttpContext context, TodoDbContext db)
        {
            if (!context.Request.RouteValues.TryGet("id", out int id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync();

            context.Response.StatusCode = 204;
        }
    }
}
