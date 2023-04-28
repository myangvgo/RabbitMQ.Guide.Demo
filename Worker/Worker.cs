using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory
{
    HostName = "192.168.26.211",
    Port = 5672,
    UserName = "sadmin",
    Password = "sadmin",
    VirtualHost = "/"
};

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

const string queueName = "task_queue";

channel.QueueDeclare(
    queue: queueName,
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

// Prevenet Dispatch messages sequentially (Round Robin)
// not to give more than one task to a worker; and make sure the worker is not busy when assigning task
channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

Console.WriteLine(" [*] Waiting for messages.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, evtArgs) => 
{
    var body = evtArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] Received message {message}");
    
    // time consuming tasks
    var dots = message.Split(".").Length - 1;
    Thread.Sleep(1000 * dots);
    Console.WriteLine(" [x] Done");

    // Add Worker Manual Ack
    channel.BasicAck(deliveryTag: evtArgs.DeliveryTag, multiple: false);
};

channel.BasicConsume(
    queue: queueName,
    autoAck: false,
    consumer: consumer
);

Console.WriteLine("Press Enter to exit.");
Console.ReadKey(false);