using PlcRabbitConsumer.MessageHandlers;
using PlcRabbitConsumer.Models;
using PlcRabbitLibrary;
using PlcRabbitLibrary.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
    .AddRabbitConnection(c =>
    {
        c.HostName = "localhost";
        c.Port = 5672;
        c.ExchangeConfigs = new List<RabbitExchangeConfig>()
        {
            new()
            {
                ExchangeName = "plc.exchange",
                QueueConfigs = new List<RabbitQueueConfig>
                {
                    new("plc.queue.product", "plc.key.product.*", "plc2.key.product.*"),
                    new("plc.queue.user", "plc.key.user.*", "plc2.key.user.*")
                }
            }
        };
    });

builder
    .Services
    .AddRabbitConsumer<Product, ProductHandler>(c =>
    {
        c.QueueName = "plc.queue.product";
    });

builder
    .Services
    .AddRabbitConsumer<User, UserHandler>(c =>
    {
        c.QueueName = "plc.queue.user";
    });

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

app.Run();
