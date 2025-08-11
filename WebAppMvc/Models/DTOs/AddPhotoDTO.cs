using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppMvc.Domain.Entities;

namespace WebAppMvc.Models.DTOs
{
    public class AddPhotoDTO
    {      

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }        

        [StringLength(200)]
        public string? Tags { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Required,Display(Name ="Album Name")]
        public int AlbumId { get; set; }    

        [Required]
        [NotMapped]
        public List<IFormFile>? Attachment { get; set; }
    }
}
