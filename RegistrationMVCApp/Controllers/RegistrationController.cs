using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RegistrationApp.Data;
using RegistrationApp.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RegistrationApp.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly DbHelper _db;

        public RegistrationController(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _db = new DbHelper(config);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List()
        {
            var data = _db.GetAllRegistrations(); // Fetch all from DB
            return View(data);                    // Pass to List.cshtml
        }

        // View for searching
        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Save(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                string filePath = null;

                if (model.Image != null && model.Image.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploads))
                        Directory.CreateDirectory(uploads);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    filePath = Path.Combine("uploads", fileName);
                    var fullPath = Path.Combine(_env.WebRootPath, filePath);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    model.ContentType = model.Image.ContentType;
                    model.ImagePath = filePath;
                }

                var success = _db.SaveRegistration(model, filePath);
                return Json(new { success = success, message = success ? "Registration successful" : "Database error" });
            }

            return Json(new
            {
                success = false,
                message = "Validation failed",
                errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                string oldFilePath = model.ImagePath; // oldFilePath: the image path already stored in the database
                string newFilePath = model.ImagePath; // newFilePath: initially assumed to be the same, unless a new image is uploaded

                if (model.Image != null && model.Image.Length > 0)
                {
                    var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsDir))
                        Directory.CreateDirectory(uploadsDir);

                    // Generate unique filename to avoid overwrites
                    string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.Image.FileName);
                    newFilePath = Path.Combine("uploads", newFileName);
                    string fullNewPath = Path.Combine(_env.WebRootPath, newFilePath);

                    // Delete old image file from wwwroot if it exists
                    if (!string.IsNullOrEmpty(oldFilePath))
                    {
                        string oldFullPath = Path.Combine(_env.WebRootPath, oldFilePath);
                        if (System.IO.File.Exists(oldFullPath))
                        {
                            System.IO.File.Delete(oldFullPath);  // Delete the old image file
                        }
                    }

                    // Save new image
                    using (var stream = new FileStream(fullNewPath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    model.ImagePath = newFilePath;
                    model.ContentType = model.Image.ContentType; // Set new content type
                }
                else
                {
                    // No new image uploaded — keep the existing ContentType and ImagePath
                    model.ImagePath = oldFilePath;
                    model.ContentType = model.ContentType; // ContentType from hidden input
                }

                var success = _db.UpdateRegistration(model, model.ImagePath);

                return Json(new
                {
                    success = success,
                    message = success ? "Record updated successfully" : "Database update failed"
                });
            }

            return Json(new
            {
                success = false,
                message = "Validation failed",
                errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
            });
        }
        // API endpoint to return matching names
        [HttpGet]
        public IActionResult SearchCandidates(string term)
        {
            var matchedCandidates = _db.SearchCandidatesByPrefix(term);

            var results = matchedCandidates
                .Select(c => new
                {
                    label = $"{c.FName} {c.LName}",
                    value = c.Id
                })
                .ToList();

            return Json(results);
        }

        [HttpGet]
        public IActionResult GetCandidatePartial(int id)
        {
            var model = _db.GetFullCandidateById(id);
            if (model == null)
                return Content($"No candidate found with ID {id}");
            return PartialView("Partials/_CandidateDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCandidateFullDetails([FromForm] CandidateFullDetailsModel model)
        {
            if (model.Registration.Id == 0)
                return Json(new { success = false, message = "Invalid candidate ID." });

            // preserve existing image metadata
            var existing = _db.GetAllRegistrations()
                              .FirstOrDefault(x => x.Id == model.Registration.Id);
            if (existing != null)
            {
                model.Registration.ImagePath = existing.ImagePath;
                model.Registration.ContentType = existing.ContentType;
            }

            bool regOk = _db.UpdateRegistration(model.Registration, model.Registration.ImagePath);
            bool detOk = _db.UpdateCandidateFullDetails(model);

            return Json(new
            {
                success = regOk && detOk,
                message = regOk && detOk
                            ? "Candidate details updated successfully."
                            : "Update failed on one or both tables."
            });
        }
    }
}