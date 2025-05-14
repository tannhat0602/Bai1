using Bai1.Models;
using System.ComponentModel.DataAnnotations;

public class FoodReview
{
    public int Id { get; set; }

    public int FoodId { get; set; }
    public Food Food { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    public string Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public int LikeCount { get; set; }

    public string? PartnerReply { get; set; }
}
