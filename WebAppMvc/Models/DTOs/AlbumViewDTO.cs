using System.ComponentModel.DataAnnotations;

namespace WebAppMvc.Models.DTOs
{
    public class AlbumViewDTO
    {
        public int Id { get; set; }        
        public string Title { get; set; }= string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime CreatedOn { get; set; }
        public int TotalPhotos { get; set; }
    }
}
