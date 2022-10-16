using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageStateManagement
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
            var queue = "queueStateMachineHeader";
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
            var queue = "queueStateMachineFanout";
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

        private static void SetUpRpc()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchangeRPC = "CodingDojoExchange_RPC";
            var queueRPC = "queueRPC";
            var queueRPCACK = "queueRPC_ACK";

            var headerRPC = new Dictionary<string, object>
            {
                { "RPC", "RPC"},
            };
            var headerRPCACK = new Dictionary<string, object>
            {
                { "RPCACK", "RPCACK"},
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchangeRPC, ExchangeType.Headers, true, false, null);
            channel.QueueDeclare(queue: queueRPC, true, false, false, null);
            channel.QueueDeclare(queue: queueRPCACK, true, false, false, null);
            channel.QueueBind(queueRPC, exchangeRPC, string.Empty, headerRPC);
            channel.QueueBind(queueRPCACK, exchangeRPC, string.Empty, headerRPCACK);

        }
    }
}
