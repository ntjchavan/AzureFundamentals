using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus
{
    public class ServiceBus2
    {
        static string connectionString = "";
        static string queue = "orders-queue";
        public static async Task SendMessage()
        {

            // Create a service bus client
            await using var client = new ServiceBusClient(connectionString);

            // Create a service bus sender
            ServiceBusSender sender = client.CreateSender(queue);

            // create a service bus message
            var message = new ServiceBusMessage(
                """
                    {
                        "orderId": 101,
                        "Amount": 2500
                    }
                """
                );
            message.ContentType = "application/json";
            message.Subject = "Order Created";
            message.MessageId = "ORDER-1001";
            message.CorrelationId = "ORDER-01";
            message.ApplicationProperties.Add("Priority", "High");

            // send the message to the queue
            await sender.SendMessageAsync(message);

            Console.WriteLine($"Sent a message to the queue {queue}");
        }

        public static async Task ReceiveMessage()
        {
            // Create a service bus client
            await using var client = new ServiceBusClient(connectionString);

            // Create a service bus receiver
            ServiceBusReceiver receiver = client.CreateReceiver(queue);

            // Receive a message from the queue
            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

            if (message != null)
            {
                Console.Write($"Received Message: {message.Body}");

                await receiver.CompleteMessageAsync(message); // complete the message after processing
            }
            else
            {
                Console.WriteLine($"No messages available in the queue {queue}.");
            }

        }
    }
}
