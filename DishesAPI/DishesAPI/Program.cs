using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using DishesAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
 builder.Configuration["ConnectionStrings:DishesDbConnectionString"]));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    Console.WriteLine($"Running at pid {System.Diagnostics.Process.GetCurrentProcess().Id}");

    return forecast.ToString() + System.Diagnostics.Process.GetCurrentProcess().Id;
});

// IMapper mapper, mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes.ToListAsync());
// ,string name 
app.MapGet("/dishes", async (DishesDbContext dishesDbContext, IMapper mapper, string? name) =>
{
    return mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes
        .Where(d => name == null || d.Name == name)
        .ToListAsync());

    // return await dishesDbContext.Dishes.ToListAsync();

});


// IMapper mapper, mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId));
app.MapGet("/dishes/{dishId:guid}", async (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId) =>
{
    return mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId));

});

// string dishName  d=> d.Name == dishName
app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, string dishName) =>
{
    return await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);

});

//mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes.Include(d => d.Ingredients)
//  .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients)
app.MapGet("/dishes/{dishId}/ingredients", async (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId) =>
{
    // .Include(d => d.Ingredients)
    return mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes
    .Include(d => d.Ingredients)
    .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients);

});

var servicescope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
var context = servicescope.ServiceProvider.GetRequiredService<DishesDbContext>();
context.Database.EnsureDeleted();
context.Database.Migrate();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
