using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppMvc.Data;
using WebAppMvc.Domain.Entities;
using WebAppMvc.Models.DTOs;

namespace WebAppMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedDataController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;

        public FeedDataController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _config = configuration;
        }

        // GET: api/FeedData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Feedback>>> GetFeedbacks()
        {
            return await _context.Feedbacks.ToListAsync();
        }       


        // GET: api/FeedData/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Feedback>> GetFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);

            if (feedback == null)
            {
                return NotFound();
            }

            return feedback;
        }

        // PUT: api/FeedData/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFeedback(int id, Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return BadRequest();
            }

            _context.Entry(feedback).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FeedData
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostFeedback")]
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
            int i= await _context.SaveChangesAsync();
            if (i > 0)
            {
                return new JsonResult(new { success = true, message = "Saved Successfully" });
            }
            else
            {
                return new JsonResult(new { success = false, message = "Failed." });
            }
        }

        // DELETE: api/FeedData/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.Id == id);
        }
    }
}

//   $.get('/api/feeddata/', function (data) {
//     let html = `<table class="table table-bordered">
//                     <thead>
//                         <tr>
//                             <th>Name</th>
//                             <th>Email</th>
//                             <th>Mobile</th>
//                             <th>Message</th>
//                             <th>File</th>
//                             <th>Date</th>
//                             <th>Actions</th>
//                         </tr>
//                     </thead>
//                     <tbody>`;

//     data.forEach(item => {
//         let fileLink = item.url ? `<a href="/uploads/${item.url}" target="_blank">Download</a>` : "No File";
//         html += `<tr>
//                     <td>${item.name}</td>
//                     <td>${item.email}</td>
//                     <td>${item.mobile}</td>
//                     <td>${item.message}</td>
//                     <td>${fileLink}</td>
//                     <td>${new Date(item.registerDate).toLocaleString()}</td>
//                     <td>
//                         <a class="btn btn-link edit-btn" data-id="${item.id}" href="#">Edit</a>|
//                         <a class="btn btn-link delete-btn" data-id="${item.id}" href="#">Delete</a>
//                     </td>
//                  </tr>`;
//     });

//     html += "</tbody></table>";
//     $('#feedbackList').html(html);
// });