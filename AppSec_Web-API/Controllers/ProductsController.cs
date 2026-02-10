using AppSec_Web_API.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppSec_Web_API.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new();

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            product.Id = _products.Count + 1;
            _products.Add(product);
            return CreatedAtAction(nameof(GetAll), product);
        }
    }
}
