using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bai1.Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 10000000)]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        // Các thuộc tính hiện có
        public string ImageUrl { get; set; } // Đường dẫn đến hình ảnh đại diện (bỏ dấu ? nếu là bắt buộc)
        public List<string>? ImageUrls { get; set; } // Danh sách các hình ảnh khác
        public List<FoodImage>? Images { get; set; } // Kiểu FoodImage cần phải hợp lệ

        public Category Category { get; set; } // Kiểu Category cần phải hợp lệ

        public int StoreId { get; set; } // Bỏ dấu ? nếu StoreId là bắt buộc

        public Store Store { get; set; }
        public ICollection<FoodReview> Reviews { get; set; } = new List<FoodReview>();
        public List<Topping> Toppings { get; set; } = new List<Topping>();
        public bool IsOutOfStock { get; set; }
    }
}
