namespace Bai1.Models
{
    public class FoodImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int FoodId { get; set; }
        public Food? Food { get; set; }
    }
}
