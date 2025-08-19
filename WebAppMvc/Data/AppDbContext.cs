using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Domain;
using WebAppMvc.Domain.Entities;

namespace WebAppMvc.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Information> Informations { get; set; }
        public DbSet<InfoType> InfoTypes { get; set; }
        public DbSet<Faculty> Faculties { get; set; }
    }
}
