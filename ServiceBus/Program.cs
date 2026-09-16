using ServiceBus;

Console.WriteLine("Hello, World!");

//await Sender.SendMessage();

//await ServiceBus2.SendMessage();
//await ServiceBus2.ReceiveMessage();


await Topics.SendMessage();
// await Topics.ReceiveMessage();
