using System.ComponentModel.DataAnnotations;

namespace Bai1.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, StringLength(50)]
        public string Name { get; set; }
        public List<Food>? Foods { get; set; }
    }
}
