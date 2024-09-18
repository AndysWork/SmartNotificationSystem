using NotificationService.Services.Implementation;
using NotificationService.Services.Interface;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Configuring EmailService
builder.Services.AddSingleton<IEmailService, EmailService>();

// Configuring RabbitMQ
builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    return factory.CreateConnection();
});

builder.Services.AddSingleton<IEventSubscriber, RabbitMqEventSubscriber>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Start event subscription service
var subscriber = app.Services.GetRequiredService<IEventSubscriber>();
subscriber.Subscribe();

app.Run();
