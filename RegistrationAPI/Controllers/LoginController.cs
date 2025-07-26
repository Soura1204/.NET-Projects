using Microsoft.AspNetCore.Mvc;
using RegistrationAPI.Helpers;
using RegistrationAPI.Models;
using RegistrationAPI.Repository;

namespace RegistrationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly DbHelper _dbHelper;

        public LoginController(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            Console.WriteLine("API Login hit. Email: " + model.Email);

            var user = _dbHelper.GetUserByEmail(model.Email);

            if (user == null || string.IsNullOrEmpty(user.Password))
            {
                Console.WriteLine("Invalid email or password");
                return Unauthorized("Invalid email/password");
            }

            bool passwordMatch = PasswordHelper.VerifyPassword(model.Password, user.Password);


            Console.WriteLine("Password match: " + passwordMatch);

            if (!passwordMatch || model.Email !=user.Email)
            {
                Console.WriteLine("Invalid email or password");
                return Unauthorized("Invalid email/password");
            }

            Console.WriteLine("Login successful");
            return Ok(user);
        }
        //public IActionResult Login([FromBody] LoginViewModel model)
        //{
        //    Console.WriteLine("API Login hit. Email: " + model.Email);

        //    var user = _dbHelper.GetUserByEmail(model.Email);

        //    if (user == null)
        //    {
        //        Console.WriteLine("User not found");
        //        return Unauthorized("Email not found");
        //    }

        //    bool passwordMatch = PasswordHelper.VerifyPassword(model.Password, user.Password);
        //    Console.WriteLine("Password match: " + passwordMatch);

        //    if (!passwordMatch)
        //        return Unauthorized("Invalid password");

        //    Console.WriteLine("Login successful");
        //    return Ok(user);
        //}
    }
}
