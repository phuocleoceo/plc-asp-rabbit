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
        c.ExchangeName = "plc.exchange";
        c.BindConfigs = new List<RabbitBindingConfig>
        {
            new("plc.queue.product", "plc.key.*", "plc2.key.*"),
            new("plc.queue.user", "plc.key.*", "plc2.key.*")
        };
    });

builder.Services.AddRabbitProducer();

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
