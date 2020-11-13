using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Todos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var app = WebApplication.Create(args);
            app.Map("/api", subapp => new TodoApi().MapRoutes(subapp));

            await app.RunAsync();
        }
    }
}
