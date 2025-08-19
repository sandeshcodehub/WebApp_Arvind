using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAppMvc.Domain;
using WebAppMvc.Domain.Entities;

namespace WebAppMvc.Data.DbSeeds
{
    public static class ContextSeed
    {
        public static async Task SeedRolesAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
            }
        }
        public static async Task SeedSuperAdminAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!userManager.Users.Any())
            {
                //Seed Default User
                var defaultUser = new AppUser
                {
                    UserName = "superadmin",
                    Email = "arvind.monu@gmail.com",
                    FirstName = "Arvind",
                    LastName = "Kumar",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    PhoneNumber = "8182823390",
                    UserType = UserProfileType.SuperAdmin
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, "Sandesh1@");
                        await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                        await userManager.AddClaimAsync(defaultUser, new Claim("UserType", UserProfileType.SuperAdmin.ToString()));
                    }
                }
            }
        }

        public static async Task SeedDbDefault(AppDbContext _context)
        {
            //if (_context.Database.EnsureCreated())
            {
                if (!_context.InfoTypes.Any())
                {
                    _context.InfoTypes.AddRange(
                        new InfoType
                        {
                            InfoTypeName = "NEWS",
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30)
                        }, new InfoType
                        {
                            InfoTypeName = "NOTICE",
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30)
                        },
                         new InfoType
                         {
                             InfoTypeName = "EVENT",
                             IsActive = true,
                             CreatedBy = "System",
                             CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30)
                         },
                        new InfoType
                        {
                            InfoTypeName = "ANNOUNCEMENT",
                            IsActive = true,
                            CreatedBy = "System",
                            CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30)
                        }
                    );
                }
                if (!_context.Faculties.Any())
                {
                    _context.Faculties.AddRange(
                    new Faculty
                    {
                        FacultyName = "Computer Science",
                        Description = "Faculty of Computer Science",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30)
                    },
                    new Faculty
                    {
                        FacultyName = "Electronics",
                        Description = "Faculty of Electronics",
                        IsActive = true,
                        CreatedBy = "System",
                        CreatedAt = DateTime.UtcNow.AddHours(5).AddMinutes(30)
                    });
                }
                await _context.SaveChangesAsync();
            }

        }

    }

}
