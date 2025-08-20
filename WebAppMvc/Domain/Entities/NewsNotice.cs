using System.ComponentModel.DataAnnotations;

namespace WebAppMvc.Domain.Entities
{
    public class Entity
    {
        [Key]
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;

        [Required]
        [StringLength(50)]
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5).AddMinutes(30);

        [StringLength(50)]
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class Information : Entity
    {       

        [Required]
        [StringLength(500)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Content { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? FileUrl { get; set; }// URL to a file associated with the information, e.g., a PDF or image
        
        [StringLength(500)]
        public string? WebUrl { get; set; } //incase when share link of external website or link
        
        [Required]
        public int InfoTypeId { get; set; }
        public InfoType InfoType { get; set; } = new InfoType();//NEWS, NOTICE, EVENT, etc.

        public int? FacultyId { get; set; }
        public Faculty? Faculty { get; set; } = null;// Faculty can be null if the information is not related to any faculty
    }

    public class InfoType : Entity
    {        
        [Required]
        [StringLength(100)]
        public string InfoTypeName { get; set; } = string.Empty;       
        public ICollection<Information> Informations { get; set; } = new List<Information>();
    }

    public class Faculty : Entity
    {     

        [Required]
        [StringLength(100)]
        public string FacultyName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }        
        public ICollection<Information> Informations { get; set; } = new List<Information>();
    }

    


    //public class InformationDto
    //{
    //    public int Id { get; set; }
    //    public string Title { get; set; } = string.Empty;
    //    public string? Content { get; set; } = string.Empty;
    //    public string? FileUrl { get; set; }
    //    public string CreatedBy { get; set; } = string.Empty;
    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5).AddMinutes(30);
    //    public bool IsActive { get; set; } = true;
    //    public string? UpdatedBy { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //    public int InfoTypeId { get; set; }
    //}
}