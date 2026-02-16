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
        private static int _nextId = 0;

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_products);
        }

        [HttpPost]
        public IActionResult Create(CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                ModelState.AddModelError("name", "Name is required.");
            }

            if (request.Price <= 0)
            {
                ModelState.AddModelError("price", "Price must be greater than zero.");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var id = Interlocked.Increment(ref _nextId);

            var product = new Product
            {
                Id = id,
                Name = request.Name,
                Price = request.Price
            };

            _products.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }


    }
}
