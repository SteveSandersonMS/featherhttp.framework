using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public static class TodoApi
    {
        public static void Attach(WebApplication app)
        {
            app.MapAction<HttpContext, TodoDbContext>("get", "/api/todos", GetAllTodos);
            app.MapAction<HttpContext, TodoDbContext, Todo>("post", "/api/todos", CreateTodo);
            app.MapAction<HttpContext, TodoDbContext, int, Todo>("post", "/api/todos/{id}", UpdateTodo);
            app.MapAction<HttpContext, TodoDbContext, int>("delete", "/api/todos/{id}", DeleteTodo);
        }

        public static async Task GetAllTodos(HttpContext http, TodoDbContext db)
        {
            var todos = await db.Todos.ToListAsync();
            await http.Response.WriteAsJsonAsync(todos);
        }

        public static async Task CreateTodo(HttpContext http, TodoDbContext db, Todo newTodo)
        {
            await db.Todos.AddAsync(newTodo);
            await db.SaveChangesAsync();

            http.Response.StatusCode = 204;
        }

        public static async Task UpdateTodo(HttpContext http, TodoDbContext db, int id, Todo updatedTodo)
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
        }

        public static async Task DeleteTodo(HttpContext http, TodoDbContext db, int id)
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
        }
    }
}
