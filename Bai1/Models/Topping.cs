using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bai1.Models
{
    public class Topping
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Range(0.01, 100000)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Price { get; set; } // Giá không bắt buộc

        public int FoodId { get; set; }

        [BindNever] // 👈 THÊM DÒNG NÀY
        public Food Food { get; set; }
    }
}