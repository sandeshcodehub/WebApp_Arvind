using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Data;
using WebAppMvc.Data.DbSeeds;
using WebAppMvc.Domain;
using WebAppMvc.Models;
using WebAppMvc.Services;

var builder = WebApplication.CreateBuilder(args);

var ConStr = builder.Configuration.GetConnectionString("ConStr") ?? throw new InvalidOperationException("Connection string 'ConStr' not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(ConStr))
                .AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
//Email Service Settings
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailSender, MailService>();//used by Identity API to send emails.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
//    try
//    {
//        var context = services.GetRequiredService<AppDbContext>();
//        var userManager = services.GetRequiredService<UserManager<AppUser>>();
//        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//        await ContextSeed.SeedRolesAsync(userManager, roleManager);
//        await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
//        await ContextSeed.SeedDbDefault(context);
//    }
//    catch (Exception ex)
//    {
//        var logger = loggerFactory.CreateLogger<Program>();
//        logger.LogError(ex, "An error occurred seeding the DB.");
//    }
//}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();//1
app.UseAuthorization();//2
app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();

app.Run();
