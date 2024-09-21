using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;
using OrderService.Services.Interface;
using OrderService.Models;
using OrderService.Data;

namespace OrderService.Services.Implementation
{
    public class RabbitMqEventSubscriber(IConnection connection, IServiceScopeFactory serviceScopeFactory) : IEventSubscriber
    {
        private readonly IConnection _connection = connection;

        public void Subscribe()
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "compensations", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var compensation = JsonSerializer.Deserialize<Compensation>(message);

                if (compensation == null) return;
                using var scope = serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
                var order = await context.Orders.FindAsync(compensation.OrderId);

                if (order == null) return;
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                Console.WriteLine($"Pedido compensado: {JsonSerializer.Serialize(order)}");
            };

            channel.BasicConsume(queue: "compensations", autoAck: true, consumer: consumer);

            Console.WriteLine("Pressione [enter] para sair.");
            Console.ReadLine();
        }
    }
}
