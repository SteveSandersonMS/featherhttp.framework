using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Todos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>();

var app = builder.Build();
app.UseDeveloperExceptionPage();

TodoApi.Attach(app);

await app.RunAsync();
