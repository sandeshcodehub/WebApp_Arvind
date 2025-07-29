using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Data;
using WebAppMvc.Domain.Entities;
using WebAppMvc.Models;
using WebAppMvc.Models.DTOs;
using X.PagedList;
using X.PagedList.Extensions;
namespace WebAppMvc.Controllers;

[Authorize(Roles = "User")]
public class UserController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IConfiguration _config;

    public UserController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _config = configuration;
    }
    public async Task<IActionResult> Index()
    {
        return View();
    }
    public IActionResult GiveFeedback()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GiveFeedback(FeedbackAddDTO feedback)
    {
        if (ModelState.IsValid)
        {
            string url = "";
            string finalpath = "";
            if (feedback.Attachment != null)
            {

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadingFolder = _config["Uploading:UserDataUpload"]!;
                string fileName = "Feedback_" + DateTime.Now.Ticks;
                string extension = Path.GetExtension(feedback.Attachment.FileName);
                url = fileName + extension;

                finalpath = Path.Combine(wwwRootPath, uploadingFolder, url);

                using (var fileStream = new FileStream(finalpath, FileMode.Create))
                {
                    await feedback.Attachment.CopyToAsync(fileStream);
                }
            }

            Feedback newFeedback = new Feedback()
            {
                Name = feedback.Name,
                Email = feedback.Email,
                Mobile = feedback.Mobile,
                Message = feedback.Message,
                Url = url,
                RegisterDate = DateTime.Now
            };


            _context.Feedbacks.Add(newFeedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(feedback);
    }


    public IActionResult FeedbackTable()
    {
        return View();
    }

    //Handle by Jquery/Ajax Operation...

    [AcceptVerbs("GET", "POST")]
    public IActionResult IsNameAvailable(string name)
    {
        bool exists = _context.Feedbacks.Any(e => e.Name == name);
        return Json(!exists);
    }

    [HttpGet]
    public IActionResult PostFeedback()
    {
        var feed = new FeedbackAddDTO();
        //ViewBag.Genders = new SelectList(_context.Genders.ToList(), "Id", "Name");
        return PartialView("_FeedbackAdd", feed);
    }

    //Add New
    [HttpPost]
    public async Task<ActionResult> PostFeedback(FeedbackAddDTO feedback)
    {
        string url = "";
        string finalpath = "";
        if (feedback.Attachment != null)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string uploadingFolder = _config["Uploading:UserDataUpload"]!;
            string fileName = "Feedback_" + DateTime.Now.Ticks;
            string extension = Path.GetExtension(feedback.Attachment.FileName);
            url = fileName + extension;
            finalpath = Path.Combine(wwwRootPath, uploadingFolder, url);
            using (var fileStream = new FileStream(finalpath, FileMode.Create))
            {
                await feedback.Attachment.CopyToAsync(fileStream);
            }
        }
        Feedback newFeedback = new Feedback()
        {
            Name = feedback.Name,
            Email = feedback.Email,
            Mobile = feedback.Mobile,
            Message = feedback.Message,
            Url = url,
            RegisterDate = DateTime.Now
        };

        _context.Feedbacks.Add(newFeedback);
        int i = await _context.SaveChangesAsync();
        if (i > 0)
        {
            return Json(new { success = true, message = "Saved Successfully" });
        }
        else
        {
            return Json(new { success = false, message = "Failed." });
        }
    }

    //List
    [HttpGet]
    public async Task<IActionResult> GetAllFeedback()
    {
        var feed = await _context.Feedbacks.ToListAsync();
        if (feed == null)
            return NotFound();
        return PartialView("_FeedbackList", feed);
    }

    //Edit/{0}
    [HttpGet]
    public IActionResult EditFeedback(int id)
    {
        var feed = _context.Feedbacks.Where(x => x.Id == id).Select(x => new FeedbackEditDTO() { Id = x.Id, Name = x.Name, Email = x.Email, Mobile = x.Mobile, Message = x.Message, Url = x.Url }).FirstOrDefault();
        if (feed == null)
            return NotFound();

        return PartialView("_FeedbackEditUpdate", feed);
    }

    //Update
    [HttpPost]
    public async Task<IActionResult> EditFeedback(int id, FeedbackEditDTO feedback)
    {
        if (id != feedback.Id)
        {
            return NotFound();
        }
        if (ModelState.IsValid)
        {
            Feedback newFeedback = _context.Feedbacks.Where(x => x.Id == id).FirstOrDefault()!;
            if (newFeedback != null)
            {
                string url = "";
                string finalpath = "";
                if (feedback.Attachment != null)
                {
                    string fileName = "Feedback_" + DateTime.Now.Ticks;
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadingFolder = _config["Uploading:UserDataUpload"]!;
                    string extension = Path.GetExtension(feedback.Attachment.FileName);
                    url = fileName + extension;
                    if (!string.IsNullOrEmpty(feedback.Url))
                    {
                        url = feedback.Url;

                        if (!string.IsNullOrEmpty(feedback.Url))
                        {
                            var delfilePath = Path.Combine(wwwRootPath, uploadingFolder, feedback.Url);
                            if (System.IO.File.Exists(delfilePath))
                            {
                                System.IO.File.Delete(delfilePath);
                            }
                        }
                    }

                    finalpath = Path.Combine(wwwRootPath, uploadingFolder, url);

                    using (var fileStream = new FileStream(finalpath, FileMode.Create))
                    {
                        await feedback.Attachment.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    url = feedback.Url!;
                }

                newFeedback.Name = feedback.Name;
                newFeedback.Email = feedback.Email;
                newFeedback.Mobile = feedback.Mobile;
                newFeedback.Message = feedback.Message;
                newFeedback.Url = url;

                _context.Update(newFeedback);
                //_context.SaveChanges();
                //return RedirectToAction("Index");
                int i = await _context.SaveChangesAsync();
                if (i > 0)
                {
                    return Json(new { success = true, message = "Update Successfully", id = newFeedback.Id });
                }
                else
                {
                    return Json(new { success = false, message = "Failed." });
                }
            }
            else
            {
                return NotFound();
            }
        }

        return PartialView("_FeedbackEditUpdate", feedback);
    }

    //Delete
    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var feedback = await _context.Feedbacks.FindAsync(id);
        if (feedback != null)
        {
            if (!string.IsNullOrEmpty(feedback.Url))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                var filePath = Path.Combine(wwwRootPath, "uploads", feedback.Url);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            _context.Feedbacks.Remove(feedback);
        }
        await _context.SaveChangesAsync();
        //return RedirectToAction(nameof(Index));
        return Json(new { success = true, message = "Deleted Successfully" });
    }

    //Datatable
    [HttpPost]
    public async Task<IActionResult> GetFeedbackTable()
    {
        var draw = Request.Form["draw"].FirstOrDefault();
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
        var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
        var searchValue = Request.Form["search[value]"].FirstOrDefault();

        int pageSize = length != null ? Convert.ToInt32(length) : 0;
        int skip = start != null ? Convert.ToInt32(start) : 0;

        var dataQuery = _context.Feedbacks.AsQueryable();

        if (!string.IsNullOrEmpty(searchValue))
        {
            dataQuery = dataQuery.Where(e => e.Name.Contains(searchValue) ||
                                             e.Email.Contains(searchValue) ||
                                             e.Mobile.Contains(searchValue));
        }

        var recordsTotal = await dataQuery.CountAsync();

        // Sorting
        if (sortColumnIndex != null)
        {
            switch (sortColumnIndex)
            {
                case "0":
                    dataQuery = sortDirection == "asc" ? dataQuery.OrderBy(e => e.Name) : dataQuery.OrderByDescending(e => e.Name);
                    break;
                case "1":
                    dataQuery = sortDirection == "asc" ? dataQuery.OrderBy(e => e.Email) : dataQuery.OrderByDescending(e => e.Email);
                    break;
                case "2":
                    dataQuery = sortDirection == "asc" ? dataQuery.OrderBy(e => e.Mobile) : dataQuery.OrderByDescending(e => e.Mobile);
                    break;
                default:
                    dataQuery = dataQuery.OrderBy(e => e.Id);
                    break;
            }
        }

        var data = await dataQuery.Skip(skip).Take(pageSize).ToListAsync();

        return Json(new
        {
            draw = draw,
            recordsFiltered = recordsTotal,
            recordsTotal = recordsTotal,
            data = data
        });
    }
    //-------Paginated List-------
    [HttpGet]
    public async Task<IActionResult> FeedbackPagedList(int? page=1)
    {
        //var feedbacks = await _context.Feedbacks
        //    .OrderByDescending(f => f.RegisterDate)
        //    .Skip((pageNumber - 1) * pageSize)
        //    .Take(pageSize)
        //    .ToListAsync();
        //var totalCount = await _context.Feedbacks.CountAsync();
        //var paginatedList = new PaginatedList<Feedback>(feedbacks, totalCount, pageNumber, pageSize);


        //int pageSize = 10;
        //var products = _context.Feedbacks.OrderBy(p => p.Name);
        //var paginatedList = await PaginatedList<Feedback>.CreateAsync(products.AsNoTracking(), pageNumber, pageSize);

        int pageSize = 1;
        var pageNumber = page ?? 1;        
        var pagedFeeds =  _context.Feedbacks.OrderBy(p => p.Name).ToPagedList(pageNumber, pageSize);
        return View(pagedFeeds);        

    }

    // AJAX endpoint for partial pagination
    [HttpGet]
    public async Task<IActionResult> FeedbackTablePaged(int? page)
    {
        int pageSize = 3;
        int pageNumber = page ?? 1;
        var feeds =  _context.Feedbacks.OrderBy(p => p.Id).ToPagedList(pageNumber, pageSize);
        return PartialView("_PagedListFeeds", feeds);
    }

    public async Task<IActionResult> IndexPaged()
    {
        return View();
    }

}

