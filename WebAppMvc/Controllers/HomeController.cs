using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Net.Mail;
using WebAppMvc.Data;
using WebAppMvc.Domain;
using WebAppMvc.Models;
using WebAppMvc.Models.DTOs;

namespace WebAppMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<AppUser> _signInManager;       
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,AppDbContext context)
        {
            _logger = logger;
            _signInManager = signInManager;           
            _userManager = userManager;
           _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(LoginDTO Input)
        {
            if (ModelState.IsValid)
            {
                var userName = Input.Email;

                if (IsValidEmail(Input.Email))
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                }
              
                var result = await _signInManager.PasswordSignInAsync(userName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    //set claim for user profile Type
                    var userType = User.Claims.Where(x => x.Type == "UserType").FirstOrDefault()!.Value;

                    if (userType == UserProfileType.User.ToString())
                    {
                        return RedirectToAction("Index", "User", new { area = "" });
                    }
                    else if (userType == UserProfileType.Admin.ToString() || userType == UserProfileType.SuperAdmin.ToString())
                    {
                        return RedirectToAction("Index", "Admin", new { area = "Secure" });
                    }
                    //return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("Index");
                }
            }

            
            return View();
        }

        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public IActionResult Info()
        {
            var InfoType = _context.InfoTypes.Where(x=>x.IsActive==true).Select(x=>new SelectListItem() { Text=x.InfoTypeName, Value=x.Id.ToString() }).ToList();
            var Faculty = _context.Faculties.Where(x => x.IsActive == true).Select(x => new SelectListItem() { Text = x.FacultyName, Value = x.Id.ToString() }).ToList();
            ViewBag.InfoTypeId = InfoType;
            ViewBag.FacultyId = Faculty;
            return View();
        }
    }
}
