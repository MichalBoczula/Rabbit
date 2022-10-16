using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace CreateInvoice.ConsumerCompetitionPattern
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumeConsumerCompetition();
        }

        private static void ConsumeConsumerCompetition()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var queue = "queueAccountantConsumerCompetition";
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
