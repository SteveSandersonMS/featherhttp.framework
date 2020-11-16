using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    public class TodoApi : IDisposable
    {
        private readonly HttpContext http;
        private readonly TodoDbContext db;

        public TodoApi(HttpContext http, TodoDbContext db)
        {
            this.http = http;
            this.db = db;
        }

        public async Task GetTodos()
        {
            var todos = await db.Todos.ToListAsync();
            await http.Response.WriteAsJsonAsync(todos);
        }

        public async Task GetTodo(int id)
        {
            var todo = await db.Todos.FindAsync(id);
            if (todo == null)
            {
                http.Response.StatusCode = 404;
                return;
            }

            await http.Response.WriteAsJsonAsync(todo);
        }

        public async Task CreateTodo()
        {
            var todo = await http.Request.ReadFromJsonAsync<Todo>();

            await db.Todos.AddAsync(todo);
            await db.SaveChangesAsync();

            http.Response.StatusCode = 204;
        }

        public async Task UpdateCompleted(int id)
        {
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

        public async Task DeleteTodo(int id)
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

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
