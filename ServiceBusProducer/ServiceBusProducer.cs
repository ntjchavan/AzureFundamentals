using Azure.Messaging.ServiceBus;
using ServiceBusMessaging.Models;
using System.Text.Json;

namespace ServiceBusMessaging
{
    public class ServiceBusProducer
    {
        public async Task SendStringMessage()
        {
            await using var client = new ServiceBusClient(Constants.connectionString);

            ServiceBusSender sender = client.CreateSender(Constants.queueName);

            var message = new ServiceBusMessage("Hello, from .Net");

            await sender.SendMessageAsync(message);

            Console.WriteLine($"Message sent to the queue {Constants.queueName}");
        }

        public async Task SendJsonMessage()
        {
            var order = new OrderCreated
            {
                OrderId = 124,
                CustomerId = 456,
                Amount = 789.99m
            };
            string json = JsonSerializer.Serialize(order);

            await using var client = new ServiceBusClient(Constants.connectionString);

            ServiceBusSender sender = client.CreateSender(Constants.queueName);

            var message = new ServiceBusMessage(json)
            {
                TimeToLive = TimeSpan.FromHours(2)
            };
            message.ContentType = "application/json"; // These are the Message Metadata properties
            message.Subject = "Order Created";
            message.MessageId = $"ORDER-{order.OrderId}";
            message.ApplicationProperties.Add("CustomerId", order.CustomerId);

            await sender.SendMessageAsync(message);

            Console.Write("Order Sent!");
        }
    }
}
