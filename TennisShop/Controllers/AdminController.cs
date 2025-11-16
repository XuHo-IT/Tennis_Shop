using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BussinessObject;
using Services;
using System.Security.Claims;
using System.Linq;
using DataAccessLayer;

namespace TennisShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly SportContext _context;

        public AdminController(IUserService userService, IProductService productService, SportContext context)
        {
            _userService = userService;
            _productService = productService;
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Debug: Check user's role
            System.Diagnostics.Debug.WriteLine($"Current user: {User.Identity.Name}");
            System.Diagnostics.Debug.WriteLine($"All roles: {string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value))}");
            
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var products = await _productService.GetAllProductsAsync();
                var orders = new List<Order>(); // You might want to add an order service later

                var dashboardViewModel = new AdminDashboardViewModel
                {
                    TotalUsers = users.Count(),
                    TotalProducts = products.Count(),
                    TotalOrders = orders.Count,
                    RecentUsers = users.OrderByDescending(u => u.CreatedAt).Take(5).ToList(),
                    RecentProducts = products.OrderByDescending(p => p.CreatedAt).Take(5).ToList()
                };

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading dashboard: " + ex.Message;
                return View(new AdminDashboardViewModel());
            }
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading users: " + ex.Message;
                return View(new List<User>());
            }
        }

        // GET: Admin/Products
        public async Task<IActionResult> Products()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading products: " + ex.Message;
                return View(new List<Product>());
            }
        }

        // GET: Admin/EditUser/5
        public async Task<IActionResult> EditUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Get all available roles
                var roles = new List<UserRole>
                {
                    new UserRole { Id = 1, Name = "Admin" },
                    new UserRole { Id = 2, Name = "User" }
                };

                ViewBag.Roles = roles;
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading user: " + ex.Message;
                return RedirectToAction("Users");
            }
        }

        // POST: Admin/EditUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            try
            {
                if (id != user.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var updatedUser = await _userService.UpdateUserAsync(user);
                    if (updatedUser != null)
                    {
                        TempData["SuccessMessage"] = "User updated successfully!";
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to update user.");
                    }
                }

                // Reload roles for the view
                var roles = new List<UserRole>
                {
                    new UserRole { Id = 1, Name = "Admin" },
                    new UserRole { Id = 2, Name = "User" }
                };
                ViewBag.Roles = roles;

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating user: " + ex.Message;
                return RedirectToAction("Users");
            }
        }

        // POST: Admin/DeleteUser/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "User deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete user.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting user: " + ex.Message;
            }

            return RedirectToAction("Users");
        }

        // POST: Admin/ChangeUserRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUserRole(int id, int roleId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Users");
                }

                user.RoleId = roleId;
                var updatedUser = await _userService.UpdateUserAsync(user);
                
                if (updatedUser != null)
                {
                    TempData["SuccessMessage"] = "User role updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update user role.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating user role: " + ex.Message;
            }

            return RedirectToAction("Users");
        }

        // GET: Admin/Debug
        public async Task<IActionResult> Debug()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var roles = _context.UserRoles.ToList();
                
                ViewBag.Users = users;
                ViewBag.Roles = roles;
                ViewBag.CurrentUser = User.Identity.Name;
                ViewBag.IsAdmin = User.IsInRole("Admin");
                ViewBag.UserClaims = User.Claims.ToList();
                
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
    }

    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public List<User> RecentUsers { get; set; } = new List<User>();
        public List<Product> RecentProducts { get; set; } = new List<Product>();
    }
}
