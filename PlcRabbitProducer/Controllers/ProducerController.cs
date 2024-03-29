using Microsoft.AspNetCore.Mvc;
using PlcRabbitLibrary.Producer;
using PlcRabbitProducer.Models;

namespace PlcRabbitProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
    private readonly IRabbitProducer<Product> _productProducer;
    private readonly IRabbitProducer<User> _userProducer;
    private readonly ILogger<ProducerController> _logger;

    public ProducerController(
        IRabbitProducer<Product> productProducer,
        IRabbitProducer<User> userProducer,
        ILogger<ProducerController> logger
    )
    {
        _productProducer = productProducer;
        _userProducer = userProducer;
        _logger = logger;
    }

    [HttpPost("Send-Product")]
    public async Task<IActionResult> SendProduct()
    {
        Product product = new Product() { Name = "Tecno Pova 4 Pro", Price = 4500000 };
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.product.v1";

        await _productProducer.ProduceAsync(exchangeName, routingKey, product);
        _logger.LogInformation(
            $"Publish message {product} to routing key: {routingKey} with exchange: {exchangeName}"
        );

        return Ok(product);
    }

    [HttpPost("Send-User")]
    public async Task<IActionResult> SendUser()
    {
        User user = new User() { Name = "Trương Minh Phước", Gender = true };
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.user.v1";

        await _userProducer.ProduceAsync(exchangeName, routingKey, user);
        _logger.LogInformation(
            $"Publish message {user} to routing key: {routingKey} with exchange: {exchangeName}"
        );

        return Ok(user);
    }
}
