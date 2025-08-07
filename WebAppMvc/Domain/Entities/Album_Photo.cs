using System.ComponentModel.DataAnnotations;

namespace WebAppMvc.Domain.Entities
{
    public class Album
    {
        [Key]
        public int Id { get; set; }


        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;


        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public ICollection<Photo>? Photos { get; set; }
    }

    public class Photo
    {
        [Key]
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required, StringLength(200)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Tags { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? UpdateOn { get; set; }
        public int AlbumId { get; set; }
        public Album Album { get; set; } = null!;
    }
}
