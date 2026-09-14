using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceBus
{
    public class Topics
    {
        static string connectionString = "";
        static string topicName = "orders-topic";

        public static async Task SendMessage()
        {
            // Create a service bus client
            await using var client = new ServiceBusClient(connectionString);

            // Create a service bus sender
            ServiceBusSender sender = client.CreateSender(topicName);

            // create a service bus message
            ServiceBusMessage message = new ServiceBusMessage(
                """
                {
                    "orderId": 201,
                    "Amount": 3000
                }
                """
                );
            message.Subject = "Order Created";
            message.ContentType = "application/json";
            message.MessageId = "ORDER-2001";
            message.CorrelationId = "ORDER-03";
            message.ApplicationProperties.Add("Priority", "High");

            // send the message to the topic
            await sender.SendMessageAsync(message);

            Console.WriteLine($"Message send to the topic {topicName}");
        }

        public static async Task ReceiveMessage()
        {
            // Create a service bus client
            await using var client = new ServiceBusClient(connectionString);

            // Create a service bus receiver for the subscription
            ServiceBusReceiver receiver = client.CreateReceiver(topicName, "notification-sub");

            // Receive a message from the subscription
            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

            if (message != null)
            {
                Console.WriteLine($"Message received: {message.Body}");

                await receiver.CompleteMessageAsync(message); // complete the message after processing
            }
            else
            {
                Console.WriteLine("No messages available in the subscription.");
            }

        }

        public static async Task ReceiveMessage1()
        {
            // Create a service bus client
            await using var client = new ServiceBusClient(connectionString);
            // Create a service bus receiver for the subscription
            ServiceBusReceiver receiver = client.CreateReceiver(topicName, "orders-subscription");
            // Receive a message from the subscription
            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();
            if (message != null)
            {
                Console.WriteLine($"Received Message: {message.Body}");
                await receiver.CompleteMessageAsync(message); // complete the message after processing
            }
            else
            {
                Console.WriteLine("No messages available in the subscription.");
            }
        }
    }
}
