using Microsoft.AspNetCore.Mvc;
using RegistrationAPI.Models;
using RegistrationAPI.Services;
using RegistrationAPI.Helpers;

namespace RegistrationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _service;
        private readonly IWebHostEnvironment _env;

        public RegistrationController(IRegistrationService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        // ---- GET ALL ----
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var data = _service.GetAll();
            return Ok(data);
        }

        // ---- GET BY ID ----
        [HttpGet("GetById/{id}")]
        public IActionResult GetById(int id)
        {
            var data = _service.GetById(id);
            if (data == null)
                return NotFound(new { success = false, message = "Record not found." });
            return Ok(data);
        }

        // ---- INSERT ----
        [HttpPost("Insert")]
        public IActionResult Insert([FromForm] RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null)
                {
                    model.ImagePath = FileHelper.SaveFile(model.Image, _env.WebRootPath);
                }

                // Hash the password before saving
                model.Password = PasswordHelper.HashPassword(model.Password);

                bool result = _service.Insert(model);
                return Ok(new { success = result, message = result ? "Inserted Successfully" : "Insert Failed" });
            }
            return BadRequest(new { success = false, message = "Validation failed", errors = ModelState });
        }

        // ---- UPDATE ----
        [HttpPut("Update")]
        public IActionResult Update([FromForm] RegistrationModel model)
        {
            if (model.Image != null)
            {
                model.ImagePath = FileHelper.SaveFile(model.Image, _env.WebRootPath);
            }

            // Optional: only hash if password is not null/empty (for updates)
            if (!string.IsNullOrEmpty(model.Password))
            {
                model.Password = PasswordHelper.HashPassword(model.Password);
            }

            bool result = _service.Update(model);
            return result ? Ok(new { success = true, imagePath = model.ImagePath })
                          : BadRequest(new { success = false });
        }       

        // ---- DELETE ----
        [HttpDelete("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            bool result = _service.Delete(id);
            return Ok(new { success = result, message = result ? "Deleted Successfully" : "Delete Failed" });
        }

        // ---- SEARCH ----
        [HttpGet("Search")]
        public IActionResult Search(string term)
        {
            if (string.IsNullOrEmpty(term))
                return Ok(new List<RegistrationModel>());  // Empty term returns empty list

            var candidates = _service.SearchByPrefix(term).ToList();
            return Ok(candidates);
        }

        // ---- GET FULL CANDIDATE DETAILS ----
        [HttpGet("GetCandidateFullDetails/{id}")]
        public IActionResult GetCandidateFullDetails(int id)
        {
            var fullDetails = _service.GetFullDetails(id); // Implement in IRegistrationService
            if (fullDetails == null)
                return NotFound(new { success = false, message = "Candidate details not found." });
            return Ok(fullDetails);
        }

        // ---- UPDATE FULL CANDIDATE DETAILS ----
        [HttpPut("UpdateFullDetails")]
        public IActionResult UpdateFullDetails([FromForm] CandidateFullDetailsModel model)
        {
            if (model.Registration.Image != null)
            {
                model.Registration.ImagePath = FileHelper.SaveFile(model.Registration.Image, _env.WebRootPath);
            }

            // Hash password if provided
            if (!string.IsNullOrEmpty(model.Registration.Password))
            {
                model.Registration.Password = PasswordHelper.HashPassword(model.Registration.Password);
            }

            bool result = _service.UpdateFullDetails(model);
            return Ok(new { success = result, message = result ? "Candidate details updated." : "Update failed." });
        }

        //[HttpPost("Login")]
        //public IActionResult Login([FromBody] LoginViewModel model)
        //{
        //    var user = _service.GetUserByEmail(model.Email);
        //    if (user != null && PasswordHelper.VerifyPassword(model.Password, user.Password))
        //    {
        //        return Ok(user); // Optionally return token here
        //    }
        //    return Unauthorized(new { success = false, message = "Invalid email or password" });
        //}
    }
}