using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrderProcessor.Worker;
using OrderProcessor.Worker.Configuration;
using OrderProcessor.Worker.Services;

Console.WriteLine("Hello, World!");


var builder = Host.CreateApplicationBuilder(args);

// This is Options Pattern, check in ServiceBusMessageProcessor class
//builder.Services.Configure<ServiceBusOptions>(builder.Configuration.GetSection("ServiceBus"));
builder.Services
    .AddOptions<ServiceBusOptions>()
    .Bind(builder.Configuration.GetSection("ServiceBus"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton(provider =>
{
    var options = provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;

    return new ServiceBusClient(options.ConnectionString);
});

builder.Services.AddSingleton(provider =>
{
    var options = provider.GetRequiredService<IOptions<ServiceBusOptions>>().Value;

    var client = provider.GetRequiredService<ServiceBusClient>();

    return client.CreateProcessor(options.QueueName, new ServiceBusProcessorOptions
    {
        AutoCompleteMessages = false,
        MaxConcurrentCalls = options.MaxConcurrentCalls,
        PrefetchCount = options.PrefetchCount
    });
});

#region Moved changes in class file ServiceBusOptions
//var connectionString = builder.Configuration["ServiceBus:ConnectionString"];

//var queueName = builder.Configuration["ServiceBus:QueueName"];

//builder.Services.AddSingleton(new ServiceBusClient(connectionString));

//builder.Services.AddSingleton(
//        serviceProvider =>
//        {
//            var client = serviceProvider.GetRequiredService<ServiceBusClient>();

//            return client.CreateProcessor(queueName, new ServiceBusProcessorOptions
//            {
//                AutoCompleteMessages = false,
//                MaxConcurrentCalls = 5,
//                PrefetchCount = 20
//            });
//        }
//    );
#endregion

builder.Services.AddScoped<IOrderProcessingService, OrderProcessingService>();

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

await app.RunAsync();
