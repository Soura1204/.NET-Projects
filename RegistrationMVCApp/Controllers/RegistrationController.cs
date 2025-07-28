using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RegistrationApp.Data;
using RegistrationApp.Helpers;
using RegistrationApp.Models;
using RegistrationApp.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RegistrationApp.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly RegistrationApiClient _apiClient;

        public RegistrationController(IWebHostEnvironment env, RegistrationApiClient apiClient)
        {
            _env = env;
            _apiClient = apiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var data = await _apiClient.GetAllRegistrationsAsync();
            return View(data);
        }

        [Authorize]
        public IActionResult Search()
        {
            return View();
        }

        // Login GET
        //public IActionResult Login()
        //{
        //    return View();
        //}

        // Login POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _apiClient.LoginAsync(model);
        //        if (user != null)
        //        {
        //            HttpContext.Session.SetString("UserEmail", user.Email); // ✅ Set session

        //            // ✅ Auth Cookie (optional if using [Authorize] without Identity)
        //            var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, user.Email)
        //    };
        //            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //            var principal = new ClaimsPrincipal(identity);
        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        //            return RedirectToAction("List", "Registration");
        //        }
        //        else
        //        {
        //            ViewBag.Message = "Invalid email or password";
        //        }
        //    }
        //    return View(model);
        //}

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); // ✅ Logs out user
            HttpContext.Session.Clear(); // ✅ Clears session variables
            TempData["SuccessMessage"] = "Logged out successfully";
            return RedirectToAction("Login", "Login"); // ✅ Ensure you're redirecting to the right controller
        }

        [HttpPost]
        public async Task<IActionResult> Save(RegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
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

            var success = await _apiClient.SaveRegistrationAsync(model);
            return Json(new { success, message = success ? "Registration successful" : "API error" });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RegistrationModel model)
        {
            var success = await _apiClient.UpdateRegistrationAsync(model);
            return Json(new { success });
        }

        [HttpGet]
        public async Task<IActionResult> SearchCandidates(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new object[] { });

            var matchedCandidates = await _apiClient.SearchCandidatesAsync(term);
            var results = matchedCandidates
                .Select(c => new
                {
                    label = $"{c.FName} {c.LName}",
                    value = c.Id
                })
                .ToList();

            return Json(results);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCandidatePartial(int id)
        {
            var model = await _apiClient.GetCandidateFullDetailsAsync(id);
            if (model == null)
                return Content($"No candidate found with ID {id}");
            return PartialView("Partials/_CandidateDetails", model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCandidateFullDetails([FromForm] CandidateFullDetailsModel model)
        {
            if (model?.Registration == null || model.Registration.Id == 0)
                return Json(new { success = false, message = "Invalid candidate ID." });

            bool success = await _apiClient.UpdateCandidateFullDetailsAsync(model);
            return Json(new
            {
                success,
                message = success
                    ? "Candidate details updated successfully."
                    : "Update failed."
            });
        }
    }
}
