//VERSIMPELD VERSIE
//WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//WebApplication app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//var Gelegenheden = new[]
//{
//    "Werk", "Studie", "Uitgaan", "Festival", "Thuis", "Verjaardag" 
//};

//app.MapGet("/kledingGelegenheden", () =>
//{
//    // De verwachte kleding is een verzameling van array met 5 kleding
//    // De verwachte kleding bij datum is vandaag + index (1 tm 5 dagen)
//    // met gelegenheden willekeurig 1 van 6 waarden
//    var verwachteKleding =  Enumerable.Range(1, 5).Select(index =>
//        new Kleding
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            Gelegenheden[Random.Shared.Next(Gelegenheden.Length)]
//        ))
//        .ToArray();
//    return verwachteKleding;
//})
//.WithName("VerkrijgVerwachteKleding")
//.WithOpenApi();

//app.MapPost();
//app.Run();

//// The name shown als result is the name of the variable in lowkey (WillekeurigeGetal = willekeurigeGetal)
//record Kleding(DateOnly Date, int WillekeurigeGetal, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(WillekeurigeGetal / 0.5556);
//}


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

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound());

app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    //adds data to the in-memory database:
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);
});

// PUT (wijzigen) requires a complete data set for the resource, whereas POST handles partial data for creating new resources
app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    Todo ?todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        //HTTP Status 204 (No Content) indicates that the server has successfully fulfilled the request
        // and that there is no content to send in the response payload body.
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();