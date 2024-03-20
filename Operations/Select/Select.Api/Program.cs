using System.Text.Json.Nodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/select", (SelectInput input) =>
    {
        Func<JsonNode, Dictionary<string, dynamic>> selector = (element) =>
        {
            var projection = new Dictionary<string, dynamic>();
            foreach (var field in input.fields)
            {
                projection.Add(field, element[field]);
                
            }
            return projection;
        };

        var result = input.data.Select(selector);
        
        return Results.Ok(result);
        
    })
    .WithName("Select")
    .WithOpenApi();

app.Run();

public partial class Program
{
}

public record SelectInput(JsonArray data, string[] fields);