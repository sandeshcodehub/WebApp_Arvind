using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppMvc.Domain.Entities;

namespace WebAppMvc.Models.DTOs
{
    public class FeedbackAddDTO
    {
        [Required, StringLength(100)]
        [Remote(action: "IsNameAvailable", controller: "User", ErrorMessage = "Name already exists")]
        public string Name { get; set; }=string.Empty;

        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }=string.Empty;

        [StringLength(10)]
        public string Mobile { get; set; }

        [Required, StringLength(2000)]
        public string Message { get; set; }=string.Empty;
        [NotMapped]
        public IFormFile? Attachment { get; set; }
    }

    public class FeedbackEditDTO
    {       
        public int Id { get; set; }

        [Required, StringLength(100)]        
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(10)]
        public string Mobile { get; set; }

        [Required, StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public string? Url { get; set; }=string.Empty;
        [NotMapped]
        public IFormFile? Attachment { get; set; }
    }
}
