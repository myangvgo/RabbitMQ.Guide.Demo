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

var exchangeName = "direct_logs";
channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

// declare a server-named queue
var queueName = channel.QueueDeclare().QueueName;

if (args.Length < 1)
{
    Console.Error.WriteLine("Usage: {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
    Console.WriteLine(" Press [Enter] to exit.");
    Console.ReadLine();
    Environment.ExitCode = 1;
    return;
}

foreach (var severity in args)
{
    channel.QueueBind(queueName, exchangeName, severity);
}

Console.WriteLine($" [*] Waiting for logs.");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, evtArgs) =>
{
    var body = evtArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var routingKey = evtArgs.RoutingKey;
    Console.WriteLine($" [x] Received '{routingKey}': '{message}'");
};

channel.BasicConsume(queueName, true, consumer);

Console.WriteLine(" Press [Enter] to exit.");
Console.ReadKey(false);