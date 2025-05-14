using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bai1.Models
{
    public class Store
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Tên cửa hàng

        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsApproved { get; set; } = false;

        [Required]
        public string OwnerId { get; set; }  // Chủ sở hữu

        [ForeignKey("OwnerId")]
        public ApplicationUser Owner { get; set; }

        [Required]
        [StringLength(100)]
        public string StreetName { get; set; }

        [Required]
        [StringLength(50)]
        public string City { get; set; }

        [Required]
        [StringLength(50)]
        public string District { get; set; }

        [Required]
        [StringLength(100)]
        public string StreetAddress { get; set; }

        [Phone]
        public string? Phone { get; set; }
        public List<Food> Foods { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsLocked { get; set; } = false;  // Khóa/mở cửa hàng
        public DateTime? UnlockDate { get; set; }    // Ngày mở khoá

    }
}
