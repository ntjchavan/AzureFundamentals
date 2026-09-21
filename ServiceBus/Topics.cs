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
        static string subscriptionName = "notification-sub";

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
            ServiceBusReceiver receiver = client.CreateReceiver(topicName, subscriptionName);

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

        public static async Task ProcessMessage()
        {
            await using var client = new ServiceBusClient(connectionString);

            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1, // Set the maximum number of concurrent calls to process messages
                AutoCompleteMessages = false, // Set to false to manually complete messages after processing
                ReceiveMode = ServiceBusReceiveMode.PeekLock, // Set the receive mode to PeekLock / ReceiveAndDelete
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5) // Set the maximum duration for auto lock renewal
            };

            // var processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions { ReceiveMode = ServiceBusReceiveMode.PeekLock });
            var processor = client.CreateProcessor(topicName, subscriptionName, options);

            processor.ProcessMessageAsync += async (args) =>
            {
                try
                {
                    var message = args.Message;

                    string body = message.Body.ToString();

                    // process the message here below completing the message

                    await args.CompleteMessageAsync(message);
                }
                catch (Exception ex)
                {
                    await args.AbandonMessageAsync(args.Message);
                    Console.WriteLine("Failed to process message", ex.ToString());
                }
            };
        }
    }
}
