using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppMvc.Domain.Entities;

namespace WebAppMvc.Models.DTOs
{
    public class AddInfoDTO
    {
        [Required]
        [Display(Name ="Category")]
        public int InfoTypeId { get; set; }
        [Display(Name = "Faculty")]
        public int? FacultyId { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        [Display(Name = "Description")]
        public string? Content { get; set; } = string.Empty;        

        [StringLength(500)]
        public string? WebUrl { get; set; }        
        
        [NotMapped]
        public IFormFile? Attachment { get; set; }
        
        [Required]
        [Display(Name = "Create Date")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5).AddMinutes(30);

    }
}
