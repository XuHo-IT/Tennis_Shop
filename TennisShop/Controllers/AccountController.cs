using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using DataAccessLayer;
using Services;
using BussinessObject;

namespace TennisShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                         Request.Headers["Accept"].ToString().Contains("application/json");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "Email and password are required." });
                }
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            try
            {
                var user = await _userService.AuthenticateUserAsync(email, password);
                if (user != null)
                {
                    // Use role name exactly as stored in database (Admin, Customer)
                    var roleName = user.Role?.Name ?? "Customer";
                    
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, roleName)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                        new ClaimsPrincipal(claimsIdentity), authProperties);

                    // Redirect based on role
                    if (user.Role?.Name?.ToLower() == "admin")
                    {
                        return RedirectToAction("Dashboard", "Admin");
                    }
                    
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = "Invalid email or password." });
                    }
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "An error occurred during login: " + ex.Message });
                }
                ModelState.AddModelError("", "An error occurred during login: " + ex.Message);
            }

            if (isAjax)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }
            return View();
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                         Request.Headers["Accept"].ToString().Contains("application/json");

            if (ModelState.IsValid)
            {
                try
                {
                    // Set default role to Customer (role ID 2)
                    user.RoleId = 2;
                    await _userService.CreateUserAsync(user);
                    
                    if (isAjax)
                    {
                        return Json(new { success = true, message = "Registration successful! Please login." });
                    }
                    TempData["SuccessMessage"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }
                catch (ArgumentException ex)
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = ex.Message });
                    }
                    ModelState.AddModelError("", ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    if (isAjax)
                    {
                        return Json(new { success = false, message = ex.Message });
                    }
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            if (isAjax)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(", ", errors) });
            }
            return View(user);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Account/Edit
        public async Task<IActionResult> Edit()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToAction("Login");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var updatedUser = await _userService.UpdateUserAsync(user);
                    if (updatedUser != null)
                    {
                        TempData["SuccessMessage"] = "Profile updated successfully!";
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update profile.");
                    }
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(user);
        }

    }
}
