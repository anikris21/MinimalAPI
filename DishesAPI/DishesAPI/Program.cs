using DishesAPI.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using DishesAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
using DishesAPI.Entities;

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

    //return forecast.ToString() + System.Diagnostics.Process.GetCurrentProcess().Id;

    return Results.Ok(forecast);
});

// IMapper mapper, mapper.Map Task<Ok<IEnumerable<DishDto>>>(await dishesDbContext.Dishes.ToListAsync());
// ,ClaimsPrincipal claimsprincipal  Task<Results<NotFound, Ok<IEnumerable<DishDto>>>> 
app.MapGet("/dishes", async Task<Results<NotFound, Ok<IEnumerable<DishDto>>>> (DishesDbContext dishesDbContext, ClaimsPrincipal claimsprincipal, IMapper mapper, string? name, string? orderby) =>
{
System.Diagnostics.Debug.WriteLine($"user authenticated? {claimsprincipal.Identity?.IsAuthenticated} {orderby}");

var res = await dishesDbContext.Dishes
    .Where(d => name == null || d.Name == name)
    .ToListAsync();

    if(res.Count ==0)
    {
        return TypedResults.NotFound();

    }

    return TypedResults.Ok( mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes
        .Where(d => name == null || d.Name == name)
        .ToListAsync()));

    // return await dishesDbContext.Dishes.ToListAsync();

});


// IMapper mapper, mapper.Map<DishDto>(await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId));
// Task<Results<NotFound, Ok<DishDto>>> 
app.MapGet("/dishes/{dishId:guid}", async Task<Results<NotFound, Ok<DishDto>>> (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId) =>
{
    var res = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if(res== null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok( mapper.Map<DishDto>(res));

}).WithName("GetDish");

// string dishName  d=> d.Name == dishName
app.MapGet("/dishes/{dishName}", async (DishesDbContext dishesDbContext, string dishName) =>
{
    return await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == dishName);

});

//mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes.Include(d => d.Ingredients)
//  .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients)
// Task<Results<NotFound, Ok<iEnumerable<IngredientDto>>>>
app.MapGet("/dishes/{dishId}/ingredients", async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>> (DishesDbContext dishesDbContext, IMapper mapper, Guid dishId) =>
{
    var dishEntity = dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id ==dishId);
    if(dishEntity== null)
    {
        return TypedResults.NotFound();
    }
    // .Include(d => d.Ingredients)
    return TypedResults.Ok( mapper.Map<IEnumerable<IngredientDto>>((await dishesDbContext.Dishes
    .Include(d => d.Ingredients)
    .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients));

});

// DishForCreationDto ,LinkGenerator link , HttpContext httpContext Task<Created<DishDto>>
app.MapPost("/dishes", async Task<CreatedAtRoute<DishDto>> (DishesDbContext dishesDbContext, 
    IMapper mapper, DishForCreationDto dishesForCreationDto, 
    LinkGenerator link, 
    HttpContext httpContext
    ) =>
{
    var entity = mapper.Map<Dish>(dishesForCreationDto);
    dishesDbContext.Add(entity);
    await dishesDbContext.SaveChangesAsync();

    var ret = mapper.Map<DishDto>(entity);

     var uri= link.GetUriByName(httpContext, "GetDish", new { dishId = ret.Id });

    //return TypedResults.Created($"https://localhost:7161/dishes/{ret.Id}", ret);

    return TypedResults.CreatedAtRoute(ret, "GetDish", new { dishId = ret.Id });

    //return TypedResults.Created(uri, ret);
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
