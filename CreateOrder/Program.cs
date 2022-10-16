using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace CreateOrder
{
    class Program
    {
        static void Main(string[] args)
        {
            SetUpConsumerCompetitionPattern();
            while (true)
            {
                Console.WriteLine("Choose actio bro;\n 1. Publish message.\n 2. Close App");
                var ele = Console.ReadLine();
                switch (ele)
                {
                    case "1":
                        Console.WriteLine("Input product name");
                        var name = Console.ReadLine();
                        Console.WriteLine("Input product quantity");
                        var msg = Console.ReadLine();
                        for(int  i = 0; i < 10; i++)
                        {
                            PublishMessageConsumerCompetitionPattern($"{name} {i}", msg);
                        }
                        break;
                    case "2":
                        Console.WriteLine("Application closed");
                        return;
                        break;
                    default:
                        Console.WriteLine("You have two option: 1 or 2.");
                        break;
                }
            }
        }

        public static void SetUpHeader()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "CodingDojoExchangeHeader";
            var queueAccountant = "queueAccountantHeader";
            var queueWarehouse = "queueWarehouseHeader";
            var queueStateMachine = "queueStateMachineHeader";
            var queueStateMachineACK = "ackRPCHeader";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Headers, true, false, null);
            channel.QueueDeclare(queue: queueAccountant, true, false, false, null);
            channel.QueueDeclare(queue: queueWarehouse, true, false, false, null);
            channel.QueueDeclare(queue: queueStateMachine, true, false, false, null);
            channel.QueueDeclare(queue: queueStateMachineACK, true, false, false, null);

            var headerAccountant = new Dictionary<string, object>
            {
                { "Accountant", "Accountant"},
            };

            var headerWarehouse = new Dictionary<string, object>
            {
                { "Warehouse", "Warehouse"},
            };

            var headerRPC = new Dictionary<string, object>
            {
                { "RPC", "RPC"},
            };

            var headerRPCACK = new Dictionary<string, object>
            {
                { "ACK", "ACK"},
            };

            channel.QueueBind(queueAccountant, exchange, string.Empty, headerAccountant);
            channel.QueueBind(queueWarehouse, exchange, string.Empty, headerWarehouse);
            channel.QueueBind(queueStateMachine, exchange, string.Empty, headerRPC);
            channel.QueueBind(queueStateMachineACK, exchange, string.Empty, headerRPCACK);
        }

        public static void PublishMessageHeader(string name, string msg)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "CodingDojoExchangeHeader";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var message = new { Name = name, Message = msg };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "Accountant", "Accountant" },
                { "Warehouse", "Warehouse" },
                { "RPC", "RPC" },
            };
            props.Persistent = true;
            var correlatinoId = Guid.NewGuid().ToString();
            props.CorrelationId = correlatinoId;
            props.ReplyTo = "ackRPCHeader";

            channel.BasicPublish(exchange: exchange,
                         routingKey: string.Empty,
                         basicProperties: props,
                         body: body);
        }

        public static void SetUpFanoutAndDirect()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "CodingDojoExchangeFanout";
            var exchange2 = "CodingDojoExchangeDirect";
            var queueAccountant = "queueAccountantFanout";
            var queueWarehouse = "queueWarehouseFanout";
            var queueStateMachine = "queueStateMachineFanout";
            var queueStateMachineACK = "ackRPCDirect";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Fanout, true, false, null);
            channel.QueueDeclare(queue: queueAccountant, true, false, false, null);
            channel.QueueDeclare(queue: queueWarehouse, true, false, false, null);
            channel.QueueDeclare(queue: queueStateMachine, true, false, false, null);
            channel.QueueDeclare(queue: queueStateMachineACK, true, false, false, null);

            channel.QueueBind(queueAccountant, exchange, string.Empty, null);
            channel.QueueBind(queueWarehouse, exchange, string.Empty, null);
            channel.QueueBind(queueStateMachine, exchange, string.Empty, null);

        }

        public static void PublishMessageFanoutAndDirect(string name, string msg)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "CodingDojoExchangeFanout";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var message = new { Name = name, Message = msg };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "Accountant", "Accountant" },
                { "Warehouse", "Warehouse" },
                { "RPC", "RPC" },
            };
            props.Persistent = true;
            var correlatinoId = Guid.NewGuid().ToString();
            props.CorrelationId = correlatinoId;
            props.ReplyTo = "ackRPCFanout";

            channel.BasicPublish(exchange: exchange,
                         routingKey: string.Empty,
                         basicProperties: props,
                         body: body);
        }

        public static void SetUpConsumerCompetitionPattern()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "CodingDojoExchangeConsumerCompetition";
            var queueAccountant = "queueAccountantConsumerCompetition";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange, ExchangeType.Headers, true, false, null);
            channel.QueueDeclare(queue: queueAccountant, true, false, false, null);
            channel.BasicQos(0, 1, true);

            var headerAccountant = new Dictionary<string, object>
            {
                { "consume", "consume"},
            };

            channel.QueueBind(queueAccountant, exchange, string.Empty, headerAccountant);
        }

        public static void PublishMessageConsumerCompetitionPattern(string name, string msg)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var exchange = "CodingDojoExchangeConsumerCompetition";

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var message = new { Name = name, Message = msg };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.Headers = new Dictionary<string, object>
            {
                { "consume", "consume"},
            };

            channel.BasicPublish(exchange: exchange,
                         routingKey: string.Empty,
                         basicProperties: props,
                         body: body);
        }
    }
}
