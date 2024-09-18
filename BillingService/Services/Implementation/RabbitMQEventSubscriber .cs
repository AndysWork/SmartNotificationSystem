using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using BillingService.Services.Interface;
using RabbitMQ.Client.Events;
using BillingService.Models;
using BillingService.Data;

namespace BillingService.Services.Implementation
{
    public class RabbitMqEventSubscriber(IConnection connection, IServiceScopeFactory serviceScopeFactory) : IEventSubscriber
    {
        private readonly IConnection _connection = connection;

        public void Subscribe()
        {
            using var _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<Order>(message);

                if (order == null)
                    return;
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<BillingDbContext>();

                var invoice = new Invoice
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductName = order.ProductName,
                    Quantity = order.Quantity,
                    Price = order.Price,
                    CreatedAt = DateTime.UtcNow
                };
                context.Invoices.Add(invoice);
                await context.SaveChangesAsync();

                Console.WriteLine($"Invoice Generated! : {JsonSerializer.Serialize(invoice)}");
            };

            _channel.BasicConsume(queue: "orders", autoAck: true, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
