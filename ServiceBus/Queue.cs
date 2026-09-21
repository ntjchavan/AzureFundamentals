using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace ServiceBus
{
    public class Queue
    {
        static string queueName = "orders";
        // Connection string to your Service Bus namespace
        static string connectionString = "";

        public static async Task SendMessage()
        {
            var orders = new
            {
                OrderID = 1,
                CustomerName = "Netaji Chavan",
                Amount = 3500
            };
            string json = JsonSerializer.Serialize(orders);


            // Create a Service Bus client
            await using var client = new ServiceBusClient(connectionString);

            // Create a Service Bus Sender for the queue
            ServiceBusSender sender = client.CreateSender(queueName);

            // Create a Service bus message
            //var message1 = new ServiceBusMessage(json)
            //{
            //    MessageId = "MSG-1001",
            //    SessionId = "ORDER-5001",
            //    CorrelationId = "REQUEST-9001"
            //};
            var message = new ServiceBusMessage(json); // you can pass string as well like this: new ServiceBusMessage("Hello, World!");
            message.ApplicationProperties.Add("Priority", "Low"); // you can add custom properties to the message
            message.MessageId = "MSG-1001"; // you can set message id
            message.TimeToLive = TimeSpan.FromMinutes(30); // you can set time to live for the message

            // Send the message
            await sender.SendMessageAsync(message);

            Console.WriteLine($"Sent a single message to the queue: {queueName}");
        }

        public static async Task ReceiveMessageProductionReady()
        {
            await using var client = new ServiceBusClient(connectionString);

            var options = new ServiceBusProcessorOptions
            {
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5), // Maximum duration for auto lock renewal
                ReceiveMode = ServiceBusReceiveMode.PeekLock, // Use PeekLock mode to process messages
                MaxConcurrentCalls = 2, // Set the maximum number of concurrent calls to process messages
                PrefetchCount = 10 // Set the number of messages to prefetch for better performance
            };

            ServiceBusProcessor procesor = client.CreateProcessor(queueName, options);

            procesor.ProcessMessageAsync += async args =>
            {
                ServiceBusReceivedMessage message = args.Message;

                try
                {

                    if (message.CorrelationId == null) // check condition & manually move message into dead letter queue
                    {
                        await args.DeadLetterMessageAsync(message, "Missing CorrelationId", "The message does not have a CorrelationId.");

                        return; // Exit the processing for this message
                    }
                    // Business logic to process the message

                    Console.WriteLine($"Received message: {message.Body}");

                    int cnt = message.DeliveryCount; // Get the delivery count of the message 

                    Console.WriteLine($"Delivery count: {cnt}");

                    await args.CompleteMessageAsync(message); // complete the message after processing
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                    await args.AbandonMessageAsync(message); // Abandon the message to make it available for reprocessing
                }
            };

            procesor.ProcessErrorAsync += async args =>
            {
                Console.WriteLine($"Error occurred: {args.Exception}");

                //return Task.CompletedTask;
            };

            await procesor.StartProcessingAsync();

            Console.WriteLine("Processor started.");

            await procesor.StopProcessingAsync();


        }

        public static async Task SessionProcessor()
        {
            var client = new ServiceBusClient(connectionString);

            ServiceBusSessionProcessor processor = client.CreateSessionProcessor("orders", new ServiceBusSessionProcessorOptions
            {
                MaxConcurrentSessions = 5,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
            });

            processor.ProcessMessageAsync += async args =>
            {
                var message = args.Message;

                Console.WriteLine($"Session: {message.SessionId}");

                Console.WriteLine($"Message: {message.MessageId}");

                Console.WriteLine($"Body: {message.Body}");

                // Add your message processing logic here
                // await ProcessMessageAsync(message);

                await args.CompleteMessageAsync(message);
            };

            processor.ProcessErrorAsync += args =>
            {
                Console.WriteLine($"Error: {args.Exception.Message}");

                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();

            Console.WriteLine("Session processor started.");

            await processor.StopProcessingAsync();
        }

    }
}
