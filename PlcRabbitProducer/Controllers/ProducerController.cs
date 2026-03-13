using Microsoft.AspNetCore.Mvc;
using PlcRabbitLibrary.Producer;
using PlcRabbitProducer.Models;

namespace PlcRabbitProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController(
    IRabbitProducer<Product> productProducer,
    IRabbitProducer<User> userProducer,
    ILogger<ProducerController> logger
) : ControllerBase
{
    [HttpPost("Send-Product")]
    public async Task<IActionResult> SendProduct()
    {
        Product product = new() { Name = "iPhone 16 Pro", Price = 22000000 };
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.product.v1";

        await productProducer.ProduceAsync(exchangeName, routingKey, product);
        logger.LogInformation(
            $"Publish message {product} to routing key: {routingKey} with exchange: {exchangeName}"
        );

        return Ok(product);
    }

    [HttpPost("Send-User")]
    public async Task<IActionResult> SendUser()
    {
        User user = new() { Name = "Trương Minh Phước", Gender = true };
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.user.v1";

        await userProducer.ProduceAsync(exchangeName, routingKey, user);
        logger.LogInformation(
            $"Publish message {user} to routing key: {routingKey} with exchange: {exchangeName}"
        );

        return Ok(user);
    }
}
