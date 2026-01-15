using System.ComponentModel.DataAnnotations;

namespace WebApplicationMVC.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }
        [Required]
        [Range(1, 120)]
        public int Age { get; set; }
    }
}
