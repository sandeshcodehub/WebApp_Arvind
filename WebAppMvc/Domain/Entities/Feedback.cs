using System.ComponentModel.DataAnnotations;

namespace WebAppMvc.Domain.Entities
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; }
        [Required, StringLength(50)]
        public string Email { get; set; }
        [StringLength(10)]
        public string Mobile { get; set; }
        [Required, StringLength(2000)]
        public string Message { get; set; }
        [StringLength(100)]
        public string? Url { get; set; }
        public DateTime RegisterDate { get; set; } = DateTime.Now;
    }


    
}
