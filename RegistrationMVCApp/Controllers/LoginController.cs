using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RegistrationApp.Models;
using RegistrationApp.Services; 
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly RegistrationApiClient _apiClient; // ✅ Add this line

        public LoginController(IHttpClientFactory httpClientFactory, RegistrationApiClient apiClient)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiClient = apiClient; // ✅ Assign here
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _apiClient.LoginAsync(model); // ✅ Now this works
                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.Email)
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    HttpContext.Session.SetString("UserEmail", user.Email);
                    // ✅ Store Email in Cookie (valid for 1 day)
                    CookieOptions options = new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(1),
                        HttpOnly = true,
                        Secure = true, // set to true if using HTTPS
                        IsEssential = true
                    };

                    // ✅ Save key properties from the registrationMVC table
                    Response.Cookies.Append("UserId", user.Id.ToString(), options);
                    Response.Cookies.Append("UserFName", user.FName, options);
                    Response.Cookies.Append("UserLName", user.LName, options);
                    Response.Cookies.Append("UserEmail", user.Email, options);
                    Response.Cookies.Append("UserPhone", user.MobileNo ?? "", options);
                    Response.Cookies.Append("UserAddress", user.Address ?? "", options);
                    Response.Cookies.Append("UserImagePath", user.ImagePath ?? "", options);

                    TempData["SuccessMessage"] = "Logged in successfully";

                    return RedirectToAction("List", "Registration");
                }
                else
                {
                    ViewBag.Message = "Invalid email or password";
                }
            }

            return View(model);
        }
    }
}
