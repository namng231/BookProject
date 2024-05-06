using BookManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly BookContext _context;
        public AuthController(BookContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            var user = _context.Users.SingleOrDefault(x => x.Username.Equals(username) && x.Password.Equals(password));
            if (user != null)
            {
                var claimsIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim("fullName", user.Name),
                }, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
                return LocalRedirect(returnUrl);
            }
            ModelState.AddModelError(string.Empty, "Username or password is invalid");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Book");
        }

        [HttpGet]
        public IActionResult GetCurrentUser()
        {
            var user = User.Identity.IsAuthenticated ? _context.Users.Find(Guid.Parse(User.Identity.Name)) : null;
            return Ok(new { user });
        }
    }
}
