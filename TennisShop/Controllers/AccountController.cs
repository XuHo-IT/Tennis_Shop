using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System;
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
            if (User.Identity.IsAuthenticated)
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
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            try
            {
                var user = await _userService.AuthenticateUserAsync(email, password);
                if (user != null)
                {
                    var isAdmin = IsAdminUser(user);
                    var roleName = isAdmin ? "Admin" : (user.Role?.Name ?? "Customer");

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

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectAfterLogin(isAdmin);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred during login: " + ex.Message);
            }

            return View();
        }

        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        // Google callback
        // Google callback
        public async Task<IActionResult> GoogleResponse()
        {
            // Lấy kết quả xác thực từ middleware Google
            var result = await HttpContext.AuthenticateAsync("Google"); // Sử dụng "Google" scheme

            if (!result.Succeeded)
                return RedirectToAction("Login");

            // Lấy thông tin claims từ Google
            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
                return BadRequest("Invalid Google user data");

            // Kiểm tra user đã có trong hệ thống chưa
            var user = await _userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                // Tạo user mới nếu chưa có
                user = new User
                {
                    FullName = name,
                    Email = email,
                    RoleId = 2, // Default Customer
                    PasswordHash = "" // Google login không cần password
                };

                // Validate riêng cho Google
                if (!await _userService.ValidateGoogleUserAsync(user))
                    return BadRequest("Invalid Google user data");

                // Tạo user mới
                await _userService.CreateUserAsync(user, isOAuthUser: true);
            }

            var isGoogleAdmin = IsAdminUser(user);

            // Tạo claims để lưu cookie login
            var appClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, isGoogleAdmin ? "Admin" : (user.Role?.Name ?? "Customer"))
    };

            var identity = new ClaimsIdentity(appClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectAfterLogin(isGoogleAdmin);
        }

        // GET: Account/Register
        [HttpGet]
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
            if (ModelState.IsValid)
            {
                try
                {
                    // Set default role to Customer (role ID 2)
                    user.RoleId = 2;
                    await _userService.CreateUserAsync(user, isOAuthUser: false);
                    TempData["SuccessMessage"] = "Registration successful! Please login.";
                    return RedirectToAction("Login");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
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
            if (!User.Identity.IsAuthenticated)
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
            if (!User.Identity.IsAuthenticated)
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

        private bool IsAdminUser(User? user)
        {
            if (user == null)
                return false;

            if (!string.IsNullOrWhiteSpace(user.Role?.Name) &&
                user.Role.Name.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return user.RoleId == 1;
        }

        private IActionResult RedirectAfterLogin(bool isAdmin)
        {
            if (isAdmin)
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
