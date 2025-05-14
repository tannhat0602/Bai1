using System.ComponentModel.DataAnnotations;

namespace Bai1.Models
{
    public class NewToppingViewModel
    {
        [Required]
        public string Name { get; set; }

        [Range(0.01, 100000)]
        public decimal? Price { get; set; }
    }

    public class CreateFoodViewModel
    {
        public Food Food { get; set; }
        public IFormFile Image { get; set; }
        public List<NewToppingViewModel> NewToppings { get; set; } = new();
    }

}
