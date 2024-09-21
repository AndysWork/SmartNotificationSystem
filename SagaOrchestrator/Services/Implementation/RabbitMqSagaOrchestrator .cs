using RabbitMQ.Client;
using SagaOrchestrator.Models;
using SagaOrchestrator.Services.Interface;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace SagaOrchestrator.Services.Implementation
{
    public class RabbitMqSagaOrchestrator(IConnection connection) : ISagaOrchestrator
    {
        private readonly IConnection _connection = connection;
        public async Task StartSagaAsync(Order order)
        {
            using var channel = _connection.CreateModel();
            var orderMessage = JsonSerializer.Serialize(order);
            var orderBody = Encoding.UTF8.GetBytes(orderMessage);
            channel.BasicPublish(exchange: "", routingKey: "orders", basicProperties: null, body: orderBody);
            Console.WriteLine($"Order published: {orderMessage}");
            // Subscribe to invoice created event
            channel.QueueDeclare(queue: "invoices", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var invoice = JsonSerializer.Deserialize<Invoice>(message);

                if (invoice == null || invoice.OrderId != order.Id) return;
                Console.WriteLine($"Invoice received: {message}");
                // Publish notification event
                var notificationMessage = JsonSerializer.Serialize(new { invoice.Id, Email = "smlgourab@gmail.com" });
                var notificationBody = Encoding.UTF8.GetBytes(notificationMessage);
                channel.BasicPublish(exchange: "", routingKey: "notifications", basicProperties: null, body: notificationBody);
                Console.WriteLine($"Notification published: {notificationMessage}");
            };

            channel.BasicConsume(queue: "invoices", autoAck: true, consumer: consumer);

            // Subscribe to compensation event
            channel.QueueDeclare(queue: "compensations", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var compensationConsumer = new EventingBasicConsumer(channel);
            compensationConsumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var compensation = JsonSerializer.Deserialize<Compensation>(message);

                if (compensation != null && compensation.OrderId == order.Id)
                {
                    Console.WriteLine($"Compensation received: {message}");
                    // Execute additional compensation logic if necessary
                }
            };

            channel.BasicConsume(queue: "compensations", autoAck: true, consumer: compensationConsumer);

            await Task.CompletedTask;
        }
    }
}
