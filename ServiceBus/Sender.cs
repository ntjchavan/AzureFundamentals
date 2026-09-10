using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace ServiceBus
{
    public class Sender
    {
        public static async Task SendMessage()
        {
            var orders = new
            {
                OrderID = 1,
                CustomerName = "Netaji Chavan",
                Amount = 3500
            };
            string json = JsonSerializer.Serialize(orders);

            string queueName = "orders";
            // Connection string to your Service Bus namespace
            string connectionString = "";

            // Create a Service Bus client
            await using var client = new ServiceBusClient(connectionString);

            // Create a Service Bus Sender for the queue
            ServiceBusSender sender = client.CreateSender(queueName);

            // Create a Service bus message
            var message = new ServiceBusMessage(json); // you can pass string as well like this: new ServiceBusMessage("Hello, World!");
            message.ApplicationProperties.Add("Priority", "Low"); // you can add custom properties to the message

            // Send the message
            await sender.SendMessageAsync(message);

            Console.WriteLine($"Sent a single message to the queue: {queueName}");
        }
    }
}
