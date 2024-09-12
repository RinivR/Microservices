using Microsoft.EntityFrameworkCore;
using MyFirstMicroservice.DatabaseContext;
using MyFirstMicroservice.Model;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//The following highlighted code adds the database context to the dependency injection (DI)
// container and enables displaying database-related exceptions:
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
//Add the Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore NuGet package.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

WebApplication app = builder.Build();

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", async (TodoDb db) =>
    await db.Todos.ToListAsync());

todoItems.MapGet("/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

todoItems.MapGet("/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    //adds data to the in-memory database:
    await db.SaveChangesAsync();

    return Results.Created($"/{todo.Id}", todo);
});

// PUT (wijzigen) requires a complete data set for the resource, whereas POST handles partial data for creating new resources
todoItems.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    Todo ?todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound("The todo item does not exist");

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.Ok($"The todo item with {id} is changed. Thank you");
});

todoItems.MapDelete("/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        //HTTP Status 204 (No Content) indicates that the server has successfully fulfilled the request
        // and that there is no content to send in the response payload body.
        return Results.Ok($"The todo item with {id} is deleted. Thank you");
    }

    return Results.NotFound($"The todo item with {id} does not exist");
});

app.Run();