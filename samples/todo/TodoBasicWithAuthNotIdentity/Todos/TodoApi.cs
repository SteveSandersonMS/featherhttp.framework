using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public class TodoApi
    {
        public void MapRoutes(WebApplication app)
        {
            app.MapGet("/api/todos", GetAllTodos).RequireAuthorization();
            app.MapGet("/api/todos/{id:int}", GetTodo).RequireAuthorization();
            app.MapPost("/api/todos", CreateTodo).RequireAuthorization();
            app.MapPost("/api/todos/{id:int}", UpdateTodo).RequireAuthorization();
            app.MapDelete("/api/todos/{id}", DeleteTodo).RequireAuthorization();
        }

        private async Task GetTodo(HttpContext http)
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
        }

        private async Task GetAllTodos(HttpContext http)
        {
            using var db = new TodoDbContext();
            var todos = await db.Todos.ToListAsync();
            await http.Response.WriteAsJsonAsync(todos);
        }

        private async Task CreateTodo(HttpContext http)
        {
            var todo = await http.Request.ReadFromJsonAsync<Todo>();

            using var db = new TodoDbContext();
            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            http.Response.StatusCode = 204;
        }

        private async Task UpdateTodo(HttpContext http)
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
        }

        private async Task DeleteTodo(HttpContext http)
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
        }
    }
}
