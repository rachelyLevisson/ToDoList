using Microsoft.AspNetCore.Mvc;
using TodoApi;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));




var app = builder.Build();
app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






app.MapGet("/item", async (ToDoDbContext db) =>
{
    try
    {
        var items = await db.Items.ToListAsync();
        return Results.Ok(items);
        System.Console.WriteLine("come!!");
    }
    catch (Exception e)
    {
        return Results.Problem("oh no!! is problem to error 500....", statusCode: 500);
    }
});


app.MapGet("/item/{id}", async (int id, ToDoDbContext db) =>
{
    try
    {
        var item = await db.Items.FindAsync(id);
        return item is not null ? Results.Ok(item) : Results.NotFound();
    }
    catch
    {
        return Results.Problem("not good!!!", statusCode: 500);
    }
});

app.MapPost("/item", async (Item item, ToDoDbContext db) =>
{
    try
    {
        db.Items.Add(item);
        await db.SaveChangesAsync();
        return Results.Created($"/api/items/{item.Id}", item);
    }
    catch
    {
        return Results.Problem("not good!!!", statusCode: 500);
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
    catch
    {
        return Results.Problem("not good!!!", statusCode: 500);
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
    catch
    {
        return Results.Problem("not good!!!", statusCode: 500);
    }
});

app.Run();
