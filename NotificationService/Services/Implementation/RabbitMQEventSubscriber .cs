using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using NotificationService.Services.Interface;
using RabbitMQ.Client.Events;
using NotificationService.Models;

namespace NotificationService.Services.Implementation
{
    public class RabbitMqEventSubscriber(IConnection connection, IEmailService emailService) : IEventSubscriber
    {
        private readonly IConnection _connection = connection;
        private readonly IEmailService _emailService = emailService;

        public void Subscribe()
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "invoices", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var invoice = JsonSerializer.Deserialize<Invoice>(message);

                if (invoice != null)
                {
                    var emailBody = $@"
                        <h1>Invoice Generated</h1>
                        <p>Order ID: {invoice.OrderId}</p>
                        <p>Product: {invoice.ProductName}</p>
                        <p>Quantity: {invoice.Quantity}</p>
                        <p>Price: {invoice.Price:C}</p>
                        <p>Total: {invoice.TotalCart:C}</p>
                        <p>Date: {invoice.CreatedAt}</p>";

                    // replace with your preferred email
                    await _emailService.SendEmailAsync("indranighoshal1@gmail.com", "Your Invoice", emailBody);
                    Console.WriteLine($"Email sent for invoice: {JsonSerializer.Serialize(invoice)}");
                }
            };

            channel.BasicConsume(queue: "invoices", autoAck: true, consumer: consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
