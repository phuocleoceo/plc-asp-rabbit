using PLCProducer;
using PLCLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

if (RabbitSettings.RabbitEnable)
{
    builder.Services.AddScoped<IProductProducer, ProductProducer>();
}

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

app.Run();