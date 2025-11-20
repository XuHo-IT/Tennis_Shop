using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BussinessObject;
using Services;
using System.Security.Claims;
using System.Linq;
using DataAccessLayer;

namespace TennisShop.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly SportManagementContext _context;
        private readonly IImageKitService _imageKitService;

        public AdminController(IUserService userService, IProductService productService, IOrderService orderService, SportManagementContext context, IImageKitService imageKitService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
            _context = context;
            _imageKitService = imageKitService;
        }

        // GET: Admin (redirect to Dashboard)
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                var products = await _productService.GetAllProductsAsync();
                var orders = await _orderService.GetAllOrdersAsync();

                var dashboardViewModel = new AdminDashboardViewModel
                {
                    TotalUsers = users.Count(),
                    TotalProducts = products.Count(),
                    TotalOrders = orders.Count(),
                    TotalRevenue = orders.Sum(o => o.TotalAmount ?? 0),
                    PendingOrders = orders.Count(o => o.Status == "pending"),
                    CompletedOrders = orders.Count(o => o.Status == "completed"),
                    RecentUsers = users.OrderByDescending(u => u.CreatedAt).Take(5).ToList(),
                    RecentProducts = products.OrderByDescending(p => p.CreatedAt).Take(5).ToList(),
                    RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList()
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

        // GET: Admin/Orders
        public async Task<IActionResult> Orders(string status = "all")
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                
                if (status != "all")
                {
                    orders = orders.Where(o => o.Status?.ToLower() == status.ToLower()).ToList();
                }

                ViewBag.SelectedStatus = status;
                return View(orders);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading orders: " + ex.Message;
                return View(new List<Order>());
            }
        }

        // GET: Admin/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound();
                }
                return View(order);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading order: " + ex.Message;
                return RedirectToAction("Orders");
            }
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(orderId, status);
                if (order != null)
                {
                    TempData["SuccessMessage"] = $"Order #{orderId} status updated to {status}!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update order status.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating order status: " + ex.Message;
            }
            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Categories()
        {
            try
            {
                var categories = await _productService.GetAllCategorysAsync();
                return View(categories);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading categories: " + ex.Message;
                return View(new List<ProductCategory>());
            }
        }

        // GET: Admin/Brands
        public async Task<IActionResult> Brands()
        {
            try
            {
                var brands = await _productService.GetAllBrandsAsync();
                return View(brands);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading brands: " + ex.Message;
                return View(new List<Brand>());
            }
        }

        // GET: Admin/Reports
        public async Task<IActionResult> Reports()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                var products = await _productService.GetAllProductsAsync();
                
                var reportViewModel = new AdminReportsViewModel
                {
                    TotalRevenue = orders.Sum(o => o.TotalAmount ?? 0),
                    TotalOrders = orders.Count(),
                    AverageOrderValue = orders.Any() ? orders.Average(o => o.TotalAmount ?? 0) : 0,
                    TopSellingProducts = products.OrderByDescending(p => p.Stock).Take(10).ToList(),
                    RevenueByMonth = orders
                        .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                        .Select(g => new MonthlyRevenue
                        {
                            Month = $"{g.Key.Year}-{g.Key.Month:00}",
                            Revenue = g.Sum(o => o.TotalAmount ?? 0)
                        })
                        .OrderBy(r => r.Month)
                        .ToList()
                };

                return View(reportViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading reports: " + ex.Message;
                return View(new AdminReportsViewModel());
            }
        }

        // POST: Admin/AddCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(string name, string? description)
        {
            try
            {
                var category = new ProductCategory
                {
                    Name = name,
                    Description = description
                };
                
                _context.ProductCategories.Add(category);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Category '{name}' added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding category: " + ex.Message;
            }
            return RedirectToAction("Categories");
        }

        // POST: Admin/EditCategory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(int id, string name, string? description)
        {
            try
            {
                var category = await _context.ProductCategories.FindAsync(id);
                if (category != null)
                {
                    category.Name = name;
                    category.Description = description;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Category '{name}' updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Category not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating category: " + ex.Message;
            }
            return RedirectToAction("Categories");
        }

        // POST: Admin/AddBrand
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBrand(string name, string? description)
        {
            try
            {
                var brand = new Brand
                {
                    Name = name,
                    Description = description
                };
                
                _context.Brands.Add(brand);
                await _context.SaveChangesAsync();
                
                TempData["SuccessMessage"] = $"Brand '{name}' added successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding brand: " + ex.Message;
            }
            return RedirectToAction("Brands");
        }

        // POST: Admin/EditBrand
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBrand(int id, string name, string? description)
        {
            try
            {
                var brand = await _context.Brands.FindAsync(id);
                if (brand != null)
                {
                    brand.Name = name;
                    brand.Description = description;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Brand '{name}' updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Brand not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating brand: " + ex.Message;
            }
            return RedirectToAction("Brands");
        }

        // GET: Admin/CreateProduct
        public async Task<IActionResult> CreateProduct()
        {
            try
            {
                var categories = await _productService.GetAllCategorysAsync();
                var brands = await _productService.GetAllBrandsAsync();
                
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading form: " + ex.Message;
                return RedirectToAction("Products");
            }
        }

        // POST: Admin/CreateProduct
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile)
        {
            try
            {
                // Initialize ProductImages collection if null
                if (product.ProductImages == null)
                {
                    product.ProductImages = new List<ProductImage>();
                }

                // Handle image upload if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{product.Name}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    using var imageStream = imageFile.OpenReadStream();
                    var imageUrl = await _imageKitService.UploadImageAsync(imageStream, fileName, "products");
                    
                    // Add the image to the product
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ImageId = ExtractImageIdFromUrl(imageUrl),
                        IsPrimary = true,
                        IsMain = true
                    });
                }

                product.CreatedAt = DateTime.Now;
                
                await _productService.CreateProductAsync(product);
                TempData["SuccessMessage"] = $"Product '{product.Name}' created successfully!";
                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error creating product: " + ex.Message;
                var categories = await _productService.GetAllCategorysAsync();
                var brands = await _productService.GetAllBrandsAsync();
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                return View(product);
            }
        }

        // GET: Admin/EditProduct/5
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Products");
                }

                var categories = await _productService.GetAllCategorysAsync();
                var brands = await _productService.GetAllBrandsAsync();
                
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;

                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading product: " + ex.Message;
                return RedirectToAction("Products");
            }
        }

        // POST: Admin/EditProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                TempData["ErrorMessage"] = "Invalid product ID.";
                return RedirectToAction("Products");
            }

            try
            {
                // Get existing product from DB
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Products");
                }

                // Handle image upload if provided
                if (imageFile != null && imageFile.Length > 0)
                {
                    var fileName = $"{product.Name}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                    using var imageStream = imageFile.OpenReadStream();
                    var imageUrl = await _imageKitService.UploadImageAsync(imageStream, fileName, "products");

                    product.ProductImages = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageUrl = imageUrl,
                            ImageId = ExtractImageIdFromUrl(imageUrl),
                            IsPrimary = true,
                            IsMain = true
                        }
                    };
                }
                else
                {
                    // Keep existing images if no new one uploaded
                    product.ProductImages = existingProduct.ProductImages?.ToList() ?? new List<ProductImage>();
                }
                
                // Update product
                var updatedProduct = await _productService.UpdateProductAsync(product);
                if (updatedProduct != null)
                {
                    TempData["SuccessMessage"] = $"Product '{product.Name}' updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update product.";
                }

                return RedirectToAction("Products");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating product: " + ex.Message;
                var categories = await _productService.GetAllCategorysAsync();
                var brands = await _productService.GetAllBrandsAsync();
                ViewBag.Categories = categories;
                ViewBag.Brands = brands;
                return View(product);
            }
        }

        // POST: Admin/DeleteProduct/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to delete product.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting product: " + ex.Message;
            }

            return RedirectToAction("Products");
        }

        // Helper method to extract image ID from URL
        private string ExtractImageIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // Extract the file path from the URL
            var uri = new Uri(imageUrl);
            return uri.AbsolutePath.TrimStart('/');
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
                ViewBag.IsAdmin = User.IsInRole("admin");
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
        public decimal TotalRevenue { get; set; }
        public int PendingOrders { get; set; }
        public int CompletedOrders { get; set; }
        public List<User> RecentUsers { get; set; } = new List<User>();
        public List<Product> RecentProducts { get; set; } = new List<Product>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }

    public class AdminReportsViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<Product> TopSellingProducts { get; set; } = new List<Product>();
        public List<MonthlyRevenue> RevenueByMonth { get; set; } = new List<MonthlyRevenue>();
    }

    public class MonthlyRevenue
    {
        public string Month { get; set; } = "";
        public decimal Revenue { get; set; }
    }
}
