using Azure.Storage.Blobs;
using AzureStorageBlob.Middleware;
using AzureStorageBlob.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetSection("AzureConnection")["BlobStorageConnection"]));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IContainerService, ContainerService>();
builder.Services.AddScoped<IBlobService, BlobService>();

builder.Services.AddScoped<ExceptionHandlingFilter>(); //Register with DI
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionHandlingFilter>(); //Apply globally
});
//if exception has handled at this step then custom middleware exception will not execute.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

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
    return forecast;
})
.WithName("GetWeatherForecast");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
