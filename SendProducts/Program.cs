using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace SendProducts
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumeFromFanoutExchange();
        }

        private static void ConsumeFromHeaderExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "queueWarehouseHeader";
            var routingKey = string.Empty;
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message}");
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);
            Console.ReadLine();
        }

        private static void ConsumeFromFanoutExchange()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "queueWarehouseFanout";
            var routingKey = string.Empty;
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Message: {message}");
            };

            channel.BasicConsume(queue: queue,
                                    autoAck: true,
                                    consumer: consumer);
            Console.ReadLine();
        }
    }
}
