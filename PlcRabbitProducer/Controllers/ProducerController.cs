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

    public ProducerController(
        IRabbitProducer<Product> productProducer,
        IRabbitProducer<User> userProducer
    )
    {
        _productProducer = productProducer;
        _userProducer = userProducer;
    }

    [HttpPost("Send-Product")]
    public async Task<IActionResult> SendProduct()
    {
        Product product = new Product() { Name = "Tecno Pova 4 Pro", Price = 4500000 };
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.product";

        await _productProducer.ProduceAsync(exchangeName, routingKey, product);
        return Ok(product);
    }

    [HttpPost("Send-User")]
    public async Task<IActionResult> SendUser()
    {
        User user = new User() { Name = "Trương Minh Phước", Gender = true };
        const string exchangeName = "plc.exchange";
        const string routingKey = "plc.key.user";

        await _userProducer.ProduceAsync(exchangeName, routingKey, user);
        return Ok(user);
    }
}
