using RabbitMQ.Client;
using OrderService.Services.Interface;
using System.Text;
using System.Text.Json;

namespace OrderService.Services.Implementation
{
    public class RabbitMqEventPublisher(IConnection connection) : IEventPublisher
    {
        private readonly IConnection _connection = connection;

        public void Publish<T>(T @event)
        {
            using var _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "orders", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "orders", basicProperties: null, body: body);
        }
    }
}
