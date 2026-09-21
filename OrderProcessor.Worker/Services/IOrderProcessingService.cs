using OrderProcessor.Worker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrderProcessor.Worker.Services
{
    public interface IOrderProcessingService
    {
        Task ProcessAsync(OrderCreated order, CancellationToken cancellationToken);
    }
}
