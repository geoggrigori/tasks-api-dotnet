using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Endpoints;
using TasksApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TasksDbContext>(options =>
    options.UseInMemoryDatabase("tasks"));

builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Tasks API",
        Version = "v1",
        Description = "A clean Minimal API for task management."
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapTaskEndpoints();

app.Run();

/// <summary>
/// Exposed so the integration test project can reference the entry point
/// via <c>WebApplicationFactory</c>.
/// </summary>
public partial class Program { }
