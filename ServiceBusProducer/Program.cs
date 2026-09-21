using ServiceBusMessaging;

Console.WriteLine("Hello, World!");

ServiceBusProducer producer = new ServiceBusProducer();

//await producer.SendStringMessage();
//await producer.SendJsonMessage();

ServiceBusConsumer consumer = new ServiceBusConsumer();
//await consumer.ReceiveManualStringMessage();
//await consumer.ReceiveManualJsonMessage();

await consumer.ReceiveContinuesStringMessage();
