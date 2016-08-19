using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Subscriber
{
    class Receive
    {
        //broker is the name of the host machine for RabbitMQ broker
        private const string _broker = "localhost";
        private const string queueName = "Ave";

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = _broker };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    //Note: Thankfully declaring a Queue is idempotent
                    //Note that we declare the queue here, as well, because we might start 
                    //the receiver before the sender, we want to make sure the queue exists
                    //before we try to consume messages from it.
                    channel.QueueDeclare(queue: queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(queue: queueName,
                                         noAck: true,
                                         consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}