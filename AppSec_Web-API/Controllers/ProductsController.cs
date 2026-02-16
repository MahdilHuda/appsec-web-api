using AppSec_Web_API.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AppSec_Web_API.DTO;

namespace AppSec_Web_API.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private static readonly List<Product> _products = new();
        private static int _nextId = 1;

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        [HttpPost]
        public IActionResult Create(CreateProductRequest request)
        {
            var id = _nextId++;
            var product = new Product
            {
                Id = id,
                Name = request.Name,
                Price = request.Price
            };

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
