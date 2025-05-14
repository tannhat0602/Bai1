using System.ComponentModel.DataAnnotations;

namespace Bai1.Models
{
    public class EditFoodViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public IFormFile? Image { get; set; }

        public string? ImageUrl { get; set; }

        public List<ToppingViewModel> Toppings { get; set; } = new();
    }

    public class ToppingViewModel
    {
        public int? Id { get; set; }  // null nếu là topping mới
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }  // dùng để xác định xóa topping
    }

}
