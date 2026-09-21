using Microsoft.Extensions.Logging;
using OrderProcessor.Worker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessor.Worker.Services
{
    public class OrderProcessingService : IOrderProcessingService
    {
        private readonly ILogger<OrderProcessingService> _logger;

        public OrderProcessingService(ILogger<OrderProcessingService> logger)
        {
            _logger = logger;
        }

        public async Task ProcessAsync(OrderCreated order, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Processing order {order.OrderId}");

            // business logic

            await Task.CompletedTask;
        }

    }
}
