using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderProcessor.Worker.Models;
using OrderProcessor.Worker.Services;
using System.Text.Json;

namespace OrderProcessor.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, ServiceBusProcessor processor, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _processor = processor;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += ProcessMessageAsync;

            _processor.ProcessErrorAsync += ProcessErrorAsync;

            _logger.LogInformation("Starting Service Bus processor.");

            await _processor.StartProcessingAsync(stoppingToken);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError("Worker stopping. " + ex.Message.ToString());
            }

            await _processor.StopProcessingAsync();

            _logger.LogInformation("Service Bus processor stopped.");
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                string body = args.Message.Body.ToString();

                _logger.LogInformation($"Message received {body}");

                OrderCreated? order = JsonSerializer.Deserialize<OrderCreated>(body);

                if (order == null)
                {
                    await args.DeadLetterMessageAsync(args.Message, "Invalid order", "Unable to deserialize order");
                    return;
                }
                // business logic
                using IServiceScope scope = _scopeFactory.CreateScope();

                IOrderProcessingService service = scope.ServiceProvider.GetRequiredService<IOrderProcessingService>();

                await service.ProcessAsync(order, args.CancellationToken);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");

                await args.AbandonMessageAsync(args.Message);
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, $"Service Bus Error. Entity {args.EntityPath}");

            return Task.CompletedTask;
        }
    }
}
