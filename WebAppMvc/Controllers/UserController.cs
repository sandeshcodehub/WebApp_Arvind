using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppMvc.Data;
using WebAppMvc.Domain.Entities;
using WebAppMvc.Models.DTOs;

namespace WebAppMvc.Controllers
{
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

        [HttpGet]
        public IActionResult EditFeedback(int id)
        {
            var feed = _context.Feedbacks.Where(x => x.Id == id).Select(x => new FeedbackEditDTO() { Id = x.Id, Name = x.Name, Email = x.Email, Mobile = x.Mobile, Message = x.Message, Url = x.Url }).FirstOrDefault();
            if (feed == null)
                return NotFound();

            return PartialView("_FeedbackEditUpdate", feed);
        }

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
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }

            return PartialView("_FeedbackEditUpdate", feedback);
        }

        [HttpPost, ActionName("Delete")]        
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
    }
}
