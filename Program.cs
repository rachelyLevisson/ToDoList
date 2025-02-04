using Microsoft.AspNetCore.Mvc;
using TodoApi;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));




var app = builder.Build();

app.UseCors("AllowAllOrigins");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapGet("/", () => "hello world!");


app.MapGet("/item", async (ToDoDbContext db) =>
{
    try
    {
        var items = await db.Items.ToListAsync();
        return Results.Ok(items);
    }
    catch (Exception e)
    {
        return Results.Problem("Error occurred: " + e.Message, statusCode: 500);
    }
});


app.MapGet("/item/{id}", async (int id, ToDoDbContext db) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        return item is not null ? Results.Ok(item) : Results.NotFound();
    }
    catch (Exception e)
    {
        return Results.Problem("Error occurred: " + e.Message, statusCode: 500);
    }
});

app.MapPost("/item", async (Item item, ToDoDbContext db) =>
{
    try
    {
        if (string.IsNullOrEmpty(item.Name))
        {
            return Results.BadRequest("Item name is required.");
        }
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return Results.Created($"/api/items/{item.Id}", item);
    }
    catch (Exception e)
    {
        return Results.Problem("Error occurred: " + e.Message, statusCode: 500);
    }
});

app.MapPut("/item/{id}", async (int id, Item item, ToDoDbContext db) =>
{
    try
    {
        if (id != item.Id) return Results.BadRequest();

        db.Entry(item).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception e)
    {
        return Results.Problem("Error occurred: " + e.Message, statusCode: 500);
    }
});

app.MapDelete("/item/{id}", async (int id, ToDoDbContext db) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        if (item is null) return Results.NotFound();

        db.Items.Remove(item);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception e)
    {

        return Results.Problem("Error occurred: " + e.Message, statusCode: 500);
    }

});

app.Run();
