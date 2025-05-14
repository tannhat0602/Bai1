using Bai1.Models;

public class UserCartItem
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int FoodId { get; set; }
    public int Quantity { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; }
    public Food Food { get; set; }
}
