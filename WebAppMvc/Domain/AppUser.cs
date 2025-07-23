using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebAppMvc.Domain
{
    public class AppUser : IdentityUser
    {
        [Required,StringLength(100),MinLength(3)]
        public string FirstName { get; set; }=string.Empty;
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        public int UsernameChangeLimit { get; set; } = 10;
        public byte[] ProfilePicture { get; set; }=Array.Empty<byte>();
        [Required]
        public UserProfileType UserType { get; set; }
        public DateOnly DateOfBirth { get; set; }
    }

}
