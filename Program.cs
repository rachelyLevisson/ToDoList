var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/Item",()=> "the list items");

app.MapGet("/Item/:Id",  () => "get item 1");

app.MapPost("/Item",()=>"the post succsess!!");

app.MapPut("/Item/:Id",()=>"the put succsess!!");

app.MapDelete("/Item/:Id",()=>"the delete succsess!!");



app.Run();
