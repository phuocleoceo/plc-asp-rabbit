using Microsoft.AspNetCore.Mvc;
using PLCLib;

namespace PLCProducer.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductProducer _productProducer;

    public ProductController(IProductProducer productProducer)
    {
        _productProducer = productProducer;
    }

    [HttpPost("Send-Product")]
    public IActionResult SendProduct()
    {
        Product product = new Product()
        {
            Name = "iPhone 6s Plus",
            Price = 2500000
        };
        _productProducer.SendProductMessage(product);
        return Ok(product);
    }
}