using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bai1.Models
{
    public class PartnerRequest
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [StringLength(100)]
        public string BusinessName { get; set; }  // Tên quán

        [Required]
        [StringLength(50)]
        public string BusinessType { get; set; }  // Loại quán (Ví dụ: Cà phê, Ăn vặt...)

        [Required]
        [StringLength(100)]
        public string StreetName { get; set; }  // Đường/Phố

        [Required]
        [StringLength(50)]
        public string City { get; set; }  // Thành phố/Tỉnh

        [Required]
        [StringLength(50)]
        public string District { get; set; }  // Quận/Huyện

        [Required]
        [StringLength(100)]
        public string StreetAddress { get; set; }  // Số nhà và tên đường/phố

        [Required]
        [Phone]
        public string BusinessPhone { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime RequestDate { get; set; } 

        public bool IsApproved { get; set; } = false;

        public DateTime? ApprovalDate { get; set; } 
        public string? ImagePath { get; set; }
    }
}
