
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

const string queueName = "rpc_queue";

channel.QueueDeclare(queueName, false, false, false, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queueName, false, consumer);

Console.WriteLine(" [x] Awaiting RPC requests.");

consumer.Received += (model, ea) => 
{
    var response = string.Empty;
    var body = ea.Body.ToArray();
    var props = ea.BasicProperties;
    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = props.CorrelationId;

    try
    {
        var message = Encoding.UTF8.GetString(body);
        var n = int.Parse(message);
        Console.WriteLine($" [.] Fib({message})");
        response = Fib(n).ToString();
    }
    catch (System.Exception ex)
    {
        Console.WriteLine($" [.] {ex.Message}");
        response = string.Empty;
    }
    finally
    {
        var responseBytes = Encoding.UTF8.GetBytes(response);
        channel.BasicPublish(string.Empty, props.ReplyTo, replyProps, responseBytes);
        channel.BasicAck(ea.DeliveryTag,false);
    }
};

Console.WriteLine(" Press [Enter] to exit");
Console.ReadLine();

static int Fib(int n)
{
    if(n is 0 or 1)
    {
        return n;
    }

    return Fib(n - 1) + Fib(n -2);
}
