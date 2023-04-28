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

var exchangeName = "logs";
channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;
channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: string.Empty);

Console.WriteLine($" [*] Waiting for logs.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, evtArgs) =>
{
    var body = evtArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($" [x] {message}");
};

channel.BasicConsume(queueName, true, consumer);

Console.WriteLine(" Press [Enter] to exit.");
Console.ReadKey(false);