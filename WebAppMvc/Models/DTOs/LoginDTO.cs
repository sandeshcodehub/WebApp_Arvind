using System.ComponentModel.DataAnnotations;

namespace WebAppMvc.Models.DTOs
{
   
    public class LoginDTO
    {        
        [Required]
        public string Email { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
