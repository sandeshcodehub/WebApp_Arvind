using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Data;
using WebAppMvc.Models.DTOs;

namespace WebAppMvc.Areas.Secure.Controllers
{
    [Area("Secure")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddInformation()
        {
            var InfoTypeList = _context.InfoTypes.Where(x => x.IsActive == true).Select(x => new SelectListItem() { Text = x.InfoTypeName, Value = x.Id.ToString() }).ToList();
            var FacultyList = _context.Faculties.Where(x => x.IsActive == true).Select(x => new SelectListItem() { Text = x.FacultyName, Value = x.Id.ToString() }).ToList();
            ViewBag.InfoTypeId = InfoTypeList;
            ViewBag.FacultyId = InfoTypeList;
            return View();
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddInformation(AddInfoDTO objEnt)
        {
            var InfoTypeList = _context.InfoTypes.Where(x => x.IsActive == true).Select(x => new SelectListItem() { Text = x.InfoTypeName, Value = x.Id.ToString() }).ToList();
            var FacultyList = _context.Faculties.Where(x => x.IsActive == true).Select(x => new SelectListItem() { Text = x.FacultyName, Value = x.Id.ToString() }).ToList();
            ViewBag.InfoTypeId = InfoTypeList;
            ViewBag.FacultyId = InfoTypeList;
            if (ModelState.IsValid)
            { 

            }
            return View();

        }

    }
}
