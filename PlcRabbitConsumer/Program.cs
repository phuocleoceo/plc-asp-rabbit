using PlcRabbitConsumer.MessageHandlers;
using PlcRabbitConsumer.Models;
using PlcRabbitLibrary;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder
    .Services
    .AddRabbitConsumer<Product, ProductHandler>(c =>
    {
        c.HostName = "localhost";
        c.Port = 5672;
        c.ExchangeName = "plc.exchange";
        c.QueueName = "plc.queue.product";
        c.RoutingKeys = new[] { "plc.key.*", "plc2.key.*" };
    });

builder
    .Services
    .AddRabbitConsumer<User, UserHandler>(c =>
    {
        c.HostName = "localhost";
        c.Port = 5672;
        c.ExchangeName = "plc.exchange";
        c.QueueName = "plc.queue.user";
        c.RoutingKeys = new[] { "plc.key.*", "plc2.key.*" };
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
