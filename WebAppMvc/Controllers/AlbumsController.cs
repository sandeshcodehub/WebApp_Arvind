using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppMvc.Data;
using WebAppMvc.Domain.Entities;
using WebAppMvc.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppMvc.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;

        public AlbumsController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _config = configuration;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Albums
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Albums.ToListAsync());
        //}
        // GET: Albums
        public async Task<IActionResult> Index()
        {
            var albums = await _context.Albums
                .Select(a => new AlbumViewDTO
                {
                    Id = a.Id,
                    Title = a.Title,
                    Description = a.Description,
                    CreatedOn = a.CreatedOn,
                    CoverImageUrl = _context.Photos.Where(p => p.AlbumId == a.Id).FirstOrDefault()!.ImageUrl
                })
                .ToListAsync();

            return View(albums);
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] Album album)
        {
            album.CreatedOn = DateTime.UtcNow.AddHours(5).AddMinutes(30); // Set CreatedOn to current time
            if (ModelState.IsValid)
            {
                _context.Add(album);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description")] Album album)
        {
            if (id != album.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }

        //---------------Photo Gallery-------------------//       

        // GET: Albums/AddPhoto
        public IActionResult AddPhoto()
        {          
            List<SelectListItem> AlbumList = _context.Albums.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Title }).ToList();
            ViewBag.AlbumList = AlbumList; // Pass the list to the view using ViewBag
            return View();
        }


        // POST: Albums/AddPhoto
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> AddPhoto([Bind("AlbumId,Title,Description,Attachment,Tags")] AddPhotoDTO objEnt)
        {
            List<SelectListItem> AlbumList = _context.Albums.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Title }).ToList();
            ViewBag.AlbumList = AlbumList; // Pass the list to the view using ViewBag 
            if (ModelState.IsValid)
            {
                string url = "";
                string finalpath = "";
                var CreatedOn = DateTime.UtcNow.AddHours(5).AddMinutes(30); // Set CreatedOn to current time
                if (objEnt.Attachment != null)
                {

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadingFolder = _config["Uploading:PhotoUpload"]!;
                    string fileName = "Photo_" + DateTime.Now.Ticks;
                    string extension = Path.GetExtension(objEnt.Attachment.FileName);
                    url = fileName + extension;

                    finalpath = Path.Combine(wwwRootPath, uploadingFolder, url);

                    using (var fileStream = new FileStream(finalpath, FileMode.Create))
                    {
                        await objEnt.Attachment.CopyToAsync(fileStream);
                    }
                    if(System.IO.File.Exists(finalpath))
                    {
                        // File successfully uploaded
                        Photo photo = new Photo
                        {
                            Title = objEnt.Title,
                            Description = objEnt.Description,
                            Tags = objEnt.Tags,
                            CreatedOn = CreatedOn,
                            ImageUrl = url,
                            AlbumId = objEnt.AlbumId
                        };

                        _context.Photos.Add(photo);
                        await _context.SaveChangesAsync();
                        TempData["msg"] = "Photo added successfully!";
                        return RedirectToAction(nameof(PhotoGallery),new { id= objEnt.AlbumId });
                        //return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // Handle file upload failure
                        TempData["msg"] = "Error: File upload failed!";
                        return View(objEnt);
                    }
                    
                }
            }
            
            return View();
        }


        // GET: Albums/PhotoGallery/5
        public IActionResult PhotoGallery(int? id)
        {
             List<Photo> Photos = _context.Photos.ToList();
            if (id != null)
            {
               Photos= Photos.Where(p => p.AlbumId == id).ToList();
            }
            
            return View(Photos);
        }

    }
}
