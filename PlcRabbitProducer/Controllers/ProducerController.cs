using Microsoft.AspNetCore.Mvc;
using PlcRabbitLibrary.Producer;
using PlcRabbitProducer.Models;

namespace PlcRabbitProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
    private readonly IRabbitProducer<Product> _productProducer;

    public ProducerController(IRabbitProducer<Product> productProducer)
    {
        _productProducer = productProducer;
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
}
