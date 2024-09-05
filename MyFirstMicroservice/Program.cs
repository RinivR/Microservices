WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var Gelegenheden = new[]
{
    "Werk", "Studie", "Uitgaan", "Festival", "Thuis", "Verjaardag" 
};

app.MapGet("/kledingGelegenheden", () =>
{
    // De verwachte kleding is een verzameling van array met 5 kleding
    // De verwachte kleding bij datum is vandaag + index (1 tm 5 dagen)
    // met gelegenheden willekeurig 1 van 6 waarden
    var verwachteKleding =  Enumerable.Range(1, 5).Select(index =>
        new Kleding
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            Gelegenheden[Random.Shared.Next(Gelegenheden.Length)]
        ))
        .ToArray();
    return verwachteKleding;
})
.WithName("VerkrijgVerwachteKleding")
.WithOpenApi();

app.MapPost()
app.Run();

// The name shown als result is the name of the variable in lowkey (WillekeurigeGetal = willekeurigeGetal)
record Kleding(DateOnly Date, int WillekeurigeGetal, string? Summary)
{
    public int TemperatureF => 32 + (int)(WillekeurigeGetal / 0.5556);
}
