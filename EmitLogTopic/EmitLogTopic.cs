﻿using System.Text;
using RabbitMQ.Client;

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

var exchangeName = "topic_logs";

// define an exchange (Topic)
channel.ExchangeDeclare(exchangeName, ExchangeType.Topic);

var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
var body = Encoding.UTF8.GetBytes(message);
channel.BasicPublish(exchangeName, routingKey, null, body);

Console.WriteLine($" [x] Sent '{routingKey}' : '{message}'");
Console.WriteLine($" Press [Enter] to exit");
Console.ReadKey();

