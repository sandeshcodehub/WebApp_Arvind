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
using X.PagedList.Extensions;
using static System.Net.Mime.MediaTypeNames;


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
                    CoverImageUrl = _context.Photos.Where(p => p.AlbumId == a.Id).FirstOrDefault()!.ImageUrl,
                    TotalPhotos= _context.Photos.Where(p => p.AlbumId == a.Id).Count()
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

            var album = await _context.Albums.FirstOrDefaultAsync(m => m.Id == id);
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

                var photos = _context.Photos.Where(p => p.AlbumId == id).ToList();

                foreach (var photo in photos)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string uploadingFolder = _config["Uploading:PhotoUpload"]!;
                    var filePath = Path.Combine(wwwRootPath, uploadingFolder, photo.ImageUrl);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    _context.Photos.Remove(photo);
                }

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

            if (objEnt.Attachment == null || objEnt.Attachment.Count == 0)
            {
                ModelState.AddModelError("", "Please select at least one image.");
                return View();
            }

            if (ModelState.IsValid)
            {
                string url = "";
                string finalpath = "";
                var CreatedOn = DateTime.UtcNow.AddHours(5).AddMinutes(30); // Set CreatedOn to current time
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadingFolder = _config["Uploading:PhotoUpload"]!;
                //if (objEnt.Attachment != null)
                {

                    foreach (var file in objEnt.Attachment)
                    {


                        string fileName = "Photo_" + DateTime.Now.Ticks;
                        string extension = Path.GetExtension(file.FileName);
                        url = fileName + extension;

                        finalpath = Path.Combine(wwwRootPath, uploadingFolder, url);

                        using (var fileStream = new FileStream(finalpath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        if (System.IO.File.Exists(finalpath))
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
                            //TempData["msg"] = "Photo added successfully!";
                            //return RedirectToAction(nameof(PhotoGallery), new { id = objEnt.AlbumId });
                            //return RedirectToAction(nameof(Index));

                        }
                        else
                        {
                            // Handle file upload failure
                            TempData["msg"] = "Error: File upload failed!";
                            return View(objEnt);
                        }
                    }
                    TempData["msg"] = "Photo added successfully!";
                    return RedirectToAction(nameof(PhotoGallery), new { id = objEnt.AlbumId });
                }
            }

            return View();
        }


        // GET: Albums/PhotoGallery/5
        //public IActionResult PhotoGallery(int? id)
        //{
        //    List<Photo> Photos = _context.Photos.ToList();
        //    ViewBag.AlbumTitle = "ALL Album"; // Initialize ViewBag.AlbumTitle
        //    if (id != null || id==0)
        //    {
        //        Photos = Photos.Where(p => p.AlbumId == id).ToList();
        //        string albumTitle = _context.Albums.Where(a => a.Id == id).Select(a => a.Title).FirstOrDefault()!;
        //        ViewBag.AlbumTitle = albumTitle; // Pass the album title to the view using ViewBag
        //    }

        //    return View(Photos);
        //}


        public IActionResult PhotoGallery(int? id, int? page = 1)
        {   
            ViewBag.AlbumTitle = "ALL Album";
            int pageSize = 8;
            var pageNumber = page ?? 1;

           var pagedPhoto = _context.Photos.OrderBy(x=>x.Id).ToPagedList(pageNumber, pageSize);

            if (id != null || id == 0)
            { 
                string albumTitle = _context.Albums.Where(a => a.Id == id).Select(a => a.Title).FirstOrDefault()!;
                ViewBag.AlbumTitle = albumTitle;
               
                pagedPhoto = _context.Photos.Where(p => p.AlbumId == id).OrderBy(x => x.Id).ToPagedList(pageNumber, pageSize);               
            }  
            return View(pagedPhoto);
        }



        // POST: Albums/PhotoDelete/5/4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PhotoDelete(int id, int? albumid)
        {

            if (id != 0)
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound();
                }
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadingFolder = _config["Uploading:PhotoUpload"]!;
                var filePath = Path.Combine(wwwRootPath, uploadingFolder, photo!.ImageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Photos.Remove(photo);
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(PhotoGallery), new { id = albumid });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelectedPhoto([FromBody] List<int> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
            {
                return BadRequest("No items selected.");
            }

            // Example: Save to database
            foreach (var id in selectedIds)
            {
                var photo = await _context.Photos.FindAsync(id);
                if (photo == null)
                {
                    return NotFound();
                }
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string uploadingFolder = _config["Uploading:PhotoUpload"]!;
                var filePath = Path.Combine(wwwRootPath, uploadingFolder, photo!.ImageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                _context.Photos.Remove(photo); 
                 await _context.SaveChangesAsync();
            }
           
            return Ok(new { message = "Photo deteled successfully", count = selectedIds.Count });
        }


    }
}
