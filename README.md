# PlcRabbitLibrary

## Introduction

PlcRabbitLibrary is a library that enables communication between ASP.NET Application and RabbitMQ.

## Installation

You can install PlcRabbitLibrary via NuGet Package Manager by running the following command in the Package Manager Console:

```bash
Install-Package PlcRabbitLibrary
```

Or use the following command in .NET CLI:

```bash
dotnet add package PlcRabbitLibrary
```

## Usage

Here's an example of how to use PlcRabbitLibrary to send and receive data between ASP.NET Application and RabbitMQ:

#### appsettings.json
```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "UserName": "username",
    "Password": "password",
    "Qos": {
      "PrefetchSize": 0,
      "PrefetchCount": 1,
      "Global": false
    },
    "Consumer": {
      "AutoAck": false,
      "AckMultiple": false
    },
    "Exchanges": [
      {
        "ExchangeName": "plc.exchange",
        "Queues": [
          {
            "QueueName": "plc.queue.product",
            "Durable": false,
            "Exclusive": false,
            "AutoDelete": false,
            "Arguments": null,
            "RoutingKeys": [
              {
                "KeyName": "plc.key.product.*",
                "Arguments": null
              },
              {
                "KeyName": "plc2.key.product.*",
                "Arguments": null
              }
            ]
          },
          {
            "QueueName": "plc.queue.user",
            "Durable": false,
            "Exclusive": false,
            "AutoDelete": false,
            "Arguments": null,
            "RoutingKeys": [
              {
                "KeyName": "plc.key.user.*",
                "Arguments": null
              },
              {
                "KeyName": "plc2.key.user.*",
                "Arguments": null
              }
            ]
          }
        ]
      }
    ]
  }
}
```

#### Producer
```csharp
using PlcRabbitLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitConnection(builder.Configuration);
builder.Services.AddRabbitProducer();
```

```csharp
using PlcRabbitLibrary.Producer;

public class YourService : IYourService
{
    private readonly IRabbitProducer<User> _userProducer;

    public YourService(IRabbitProducer<User> userProducer)
    {
        _userProducer = userProducer;
    }

    public async Task SendUserMessage(User user)
    {
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.user.v1";
        await _userProducer.ProduceAsync(exchangeName, routingKey, user);
    }
```

#### Consumer
```csharp
using PlcRabbitLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRabbitConnection(builder.Configuration);
builder.Services.AddRabbitConsumer<Product, ProductHandler>();
builder.Services.AddRabbitConsumer<User, UserHandler>();
```

```csharp
using PlcRabbitLibrary.Consumer;

public class UserEventHandler : IRabbitConsumerHandler<User>
{
    public string QueueName => "plc.queue.user";

    public async Task HandleAsync(User user)
    {
        **... your logic ...**
        await Task.CompletedTask;
    }
}
```

## Contribution

Contributions are welcome! If you'd like to contribute to PlcRabbitLibrary, please create an issue or send a pull request on our GitHub repository.

## License

PlcRabbitLibrary is distributed under the [MIT License](LICENSE). See the LICENSE file for more information.

## Contact

If you have any questions or suggestions, feel free to contact me via email: phuoc.truong.1008@gmail.com

--- 

The library is developed by phuocleoceo - Copyright © 2024
