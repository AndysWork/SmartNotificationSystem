using RabbitMQ.Client;
using SmartNotificationService.Services.Implementation;
using SmartNotificationService.Services.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory() { HostName = "localhost" };
    return factory.CreateConnection();
});

builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();


builder.Services.AddControllers();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = "https://your-auth-server",
//            ValidAudience = "https://your-audience"
//        };

//    });
//builder.Services.AddMemoryCache();
//builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.MapHub<NotificationHub>("/notificationHub");

app.Run();
