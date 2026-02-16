using System.ComponentModel.DataAnnotations;

namespace AppSec_Web_API.DTO
{
    public class CreateProductRequest
    {
        [Required]
        public string Name { get; set; }

        [Range(0.01, 1_000_000)]
        public decimal Price { get; set; }
    }
}
