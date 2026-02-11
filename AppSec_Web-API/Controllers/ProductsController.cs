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
            _products.Add(product);
            return CreatedAtAction(nameof(GetAll), product);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _products.First(p => p.Id == id); // kaster exception hvis ikke fundet
            return Ok(product);
        }

    }
}
