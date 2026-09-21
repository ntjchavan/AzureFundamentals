using Azure.Messaging.ServiceBus;
using ServiceBusMessaging.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace ServiceBusMessaging
{
    public class ServiceBusConsumer
    {
        public async Task ReceiveManualStringMessage()
        {
            await using var client = new ServiceBusClient(Constants.connectionString);

            ServiceBusReceiver receiver = client.CreateReceiver(Constants.queueName);

            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

            if (message is not null)
            {
                Console.WriteLine($"Received Message: {message.Body}");

                await receiver.CompleteMessageAsync(message);
            }
        }

        public async Task ReceiveManualJsonMessage()
        {
            await using var client = new ServiceBusClient(Constants.connectionString);

            ServiceBusReceiver receiver = client.CreateReceiver(Constants.queueName);

            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

            if (message is not null)
            {
                if (message.ContentType != "text/plain")
                {
                    OrderCreated? orders = JsonSerializer.Deserialize<OrderCreated>(message.Body);

                    Console.WriteLine($"Received Message: OrderID-{orders?.OrderId}, CustomerID{orders?.CustomerId}, Amount{orders?.Amount}");
                }
                else
                {
                    Console.WriteLine($"Received Message: {message.Body}");
                }
                await receiver.CompleteMessageAsync(message);

            }
        }

        public async Task ReceiveContinuesStringMessage()
        {
            await using var client = new ServiceBusClient(Constants.connectionString);

            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 2,
                PrefetchCount = 5,
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = false,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(2)
            };

            ServiceBusProcessor processor = client.CreateProcessor(Constants.queueName, options);

            processor.ProcessMessageAsync += MessageStringHandler;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();

            Console.WriteLine("Processor Started!");

            Console.WriteLine("Press enter to stop");
            Console.ReadLine();

            await processor.StopProcessingAsync();

            Console.WriteLine("Processor Stopped!");
        }

        private async Task MessageStringHandler(ProcessMessageEventArgs args)
        {
            try
            {
                string body = args.Message.Body.ToString();

                Console.WriteLine($"Received Message: {body}");

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Processing Failed, {ex.Message}");
                await args.AbandonMessageAsync(args.Message);
            }
        }

        private async Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occured! {args.Exception.Message}");

            await Task.CompletedTask;
        }

    }
}
