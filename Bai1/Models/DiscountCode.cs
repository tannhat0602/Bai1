using System;

namespace Bai1.Models
{
    public class DiscountCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public decimal DiscountAmount { get; set; }

        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow;

        public int MaxUsageCount { get; set; }

        public int StoreId { get; set; } // Liên kết đến Store
        public Store Store { get; set; } // (tuỳ chọn)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
