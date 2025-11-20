using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System.Security.Claims;

namespace TennisShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IImageKitService _imageKitService;
        private readonly IProductReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public ProductController(IProductService productService, IImageKitService imageKitService, IProductReviewService reviewService, IOrderService orderService, IUserService userService)
        {
            _productService = productService;
            _imageKitService = imageKitService;
            _reviewService = reviewService;
            _orderService = orderService;
            _userService = userService;
        }

        // GET: Product
        public async Task<IActionResult> Index(int? categoryId, string sortBy = "newest")
        {
            IEnumerable<Product> products;
            
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                products = await _productService.GetProductsByCategoryAsync(categoryId.Value);
                ViewBag.SelectedCategoryId = categoryId.Value;
            }
            else
            {
                products = await _productService.GetAllProductsAsync();
            }
            
            // Apply sorting
            products = SortProducts(products, sortBy);
            
            // Load categories for navigation
            var categories = await _productService.GetAllCategorysAsync();
            ViewBag.Categories = categories;
            ViewBag.SortBy = sortBy;
            
            return View(products);
        }
        
        private IEnumerable<Product> SortProducts(IEnumerable<Product> products, string sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "newest" => products.OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue),
                "oldest" => products.OrderBy(p => p.CreatedAt ?? DateTime.MinValue),
                "pricelow" => products.OrderBy(p => p.BasePrice),
                "pricehigh" => products.OrderByDescending(p => p.BasePrice),
                "nameaz" => products.OrderBy(p => p.Name),
                "nameza" => products.OrderByDescending(p => p.Name),
                _ => products.OrderByDescending(p => p.CreatedAt ?? DateTime.MinValue)
            };
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Load reviews
            var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
            ViewBag.Reviews = reviews;

            // Check if current user can review (has purchased the product)
            bool canReview = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(userIdString, out int userId))
                {
                    canReview = await _orderService.HasUserPurchasedProductAsync(userId, id);
                }
            }
            ViewBag.CanReview = canReview;

            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadBrandsAndCategories();
            return View();
        }
        
        private async Task LoadBrandsAndCategories()
        {
            var brands = await _productService.GetAllBrandsAsync();
            var categories = await _productService.GetAllCategorysAsync();
            
            ViewBag.Brands = new SelectList(brands, "Id", "Name");
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Initialize ProductImages collection if null
                    if (product.ProductImages == null)
                    {
                        product.ProductImages = new List<ProductImage>();
                    }

                    // Initialize ProductVariants collection if null
                    if (product.ProductVariants == null)
                    {
                        product.ProductVariants = new List<ProductVariant>();
                    }

                    // Handle variants from form
                    var variants = Request.Form["ProductVariants"];
                    if (variants.Count > 0)
                    {
                        product.ProductVariants = new List<ProductVariant>();
                        var variantIndices = new HashSet<int>();
                        
                        // Collect all variant indices
                        foreach (var key in Request.Form.Keys)
                        {
                            if (key.StartsWith("ProductVariants[") && key.Contains("].Color"))
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(key, @"ProductVariants\[(\d+)\]");
                                if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                                {
                                    variantIndices.Add(index);
                                }
                            }
                        }

                        // Process each variant
                        foreach (var index in variantIndices)
                        {
                            var color = Request.Form[$"ProductVariants[{index}].Color"].ToString();
                            var size = Request.Form[$"ProductVariants[{index}].Size"].ToString();
                            var priceStr = Request.Form[$"ProductVariants[{index}].Price"].ToString();
                            var stockStr = Request.Form[$"ProductVariants[{index}].Stock"].ToString();
                            var sku = Request.Form[$"ProductVariants[{index}].Sku"].ToString();

                            // Only add variant if at least color or size is provided
                            if (!string.IsNullOrWhiteSpace(color) || !string.IsNullOrWhiteSpace(size))
                            {
                                var variant = new ProductVariant
                                {
                                    Color = string.IsNullOrWhiteSpace(color) ? null : color,
                                    Size = string.IsNullOrWhiteSpace(size) ? null : size,
                                    Price = decimal.TryParse(priceStr, out decimal price) ? price : null,
                                    Stock = int.TryParse(stockStr, out int stock) ? stock : 0,
                                    Sku = string.IsNullOrWhiteSpace(sku) ? null : sku
                                };
                                product.ProductVariants.Add(variant);
                            }
                        }
                    }

                    // Handle image upload if provided
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var fileName = $"{product.Name}_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                        using var imageStream = imageFile.OpenReadStream();
                        var imageId = await _imageKitService.UploadImageAsync(imageStream, fileName, "products");
                        
                        // Add the image to the product
                        product.ProductImages.Add(new ProductImage
                        {
                            ImageUrl = imageId, // imageId is actually the URL returned from UploadImageAsync
                            ImageId = ExtractImageIdFromUrl(imageId),
                            IsPrimary = true,
                            IsMain = true
                        });
                    }

                    await _productService.CreateProductAsync(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error uploading image: {ex.Message}");
                }
            }
            await LoadBrandsAndCategories();
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = (await _productService.GetAllCategorysAsync()) ?? new List<ProductCategory>();
            var brands = (await _productService.GetAllBrandsAsync()) ?? new List<Brand>();

            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(brands, "Id", "Name", product.BrandId);


            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                ViewBag.Categories = new SelectList(await _productService.GetAllCategorysAsync(), "Id", "Name", product.CategoryId);
                ViewBag.Brands = new SelectList(await _productService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
                return View(product);
            }

            try
            {
                // Get existing product from DB
                var existingProduct = await _productService.GetProductByIdAsync(id);
                if (existingProduct == null)
                    return NotFound();

                // Initialize ProductVariants collection
                product.ProductVariants = new List<ProductVariant>();

                // Handle variants from form
                var variantIndices = new HashSet<int>();
                
                // Collect all variant indices
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("ProductVariants[") && key.Contains("].Color"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(key, @"ProductVariants\[(\d+)\]");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int index))
                        {
                            variantIndices.Add(index);
                        }
                    }
                }

                // Process each variant
                foreach (var index in variantIndices)
                {
                    var variantIdStr = Request.Form[$"ProductVariants[{index}].Id"].ToString();
                    var color = Request.Form[$"ProductVariants[{index}].Color"].ToString();
                    var size = Request.Form[$"ProductVariants[{index}].Size"].ToString();
                    var priceStr = Request.Form[$"ProductVariants[{index}].Price"].ToString();
                    var stockStr = Request.Form[$"ProductVariants[{index}].Stock"].ToString();
                    var sku = Request.Form[$"ProductVariants[{index}].Sku"].ToString();

                    // Only add variant if at least color or size is provided
                    if (!string.IsNullOrWhiteSpace(color) || !string.IsNullOrWhiteSpace(size))
                    {
                        var variant = new ProductVariant
                        {
                            Id = int.TryParse(variantIdStr, out int variantId) && variantId > 0 ? variantId : 0,
                            ProductId = id,
                            Color = string.IsNullOrWhiteSpace(color) ? null : color,
                            Size = string.IsNullOrWhiteSpace(size) ? null : size,
                            Price = decimal.TryParse(priceStr, out decimal price) ? price : null,
                            Stock = int.TryParse(stockStr, out int stock) ? stock : 0,
                            Sku = string.IsNullOrWhiteSpace(sku) ? null : sku
                        };
                        product.ProductVariants.Add(variant);
                    }
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
                if (updatedProduct == null)
                    return NotFound();

                TempData["SuccessMessage"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
            }

            // Repopulate dropdowns if update failed
            ViewBag.Categories = new SelectList(await _productService.GetAllCategorysAsync(), "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(await _productService.GetAllBrandsAsync(), "Id", "Name", product.BrandId);
            return View(product);
        }


        // GET: Product/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Product/Search
        public async Task<IActionResult> Search(string searchTerm, string sortBy = "newest")
        {
            IEnumerable<Product> products;
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                products = await _productService.GetAllProductsAsync();
            }
            else
            {
                products = await _productService.SearchProductsAsync(searchTerm);
            }
            
            // Apply sorting
            products = SortProducts(products, sortBy);
            
            // Load categories for navigation
            var categories = await _productService.GetAllCategorysAsync();
            ViewBag.Categories = categories;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            
            return View("Index", products);
        }

        private string ExtractImageIdFromUrl(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return string.Empty;

            // Extract the file path from the URL
            // Example: https://ik.imagekit.io/6tazphtet3/products/ProductName_20231201120000.jpg
            // Should return: products/ProductName_20231201120000.jpg
            var uri = new Uri(imageUrl);
            return uri.AbsolutePath.TrimStart('/');
        }


        [HttpPost]
        [Authorize] // Chỉ cho phép user đã đăng nhập
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            try
            {
                // Lấy thông tin user hiện tại
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = User.Identity.Name;

                // Convert userId từ string sang int
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid user ID"
                    });
                }

                // Kiểm tra user đã mua sản phẩm chưa
                bool hasPurchased = await _orderService.HasUserPurchasedProductAsync(userId, productId);
                if (!hasPurchased)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bạn chỉ có thể đánh giá sản phẩm sau khi đã mua. Vui lòng mua sản phẩm này trước khi đánh giá."
                    });
                }

                // Kiểm tra user đã review sản phẩm này chưa
                var existingReviews = await _reviewService.GetReviewsByProductIdAsync(productId);
                if (existingReviews.Any(r => r.user_id == userId))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Bạn đã đánh giá sản phẩm này rồi. Mỗi khách hàng chỉ có thể đánh giá một lần."
                    });
                }

                // Lấy thông tin user để lấy full name
                var user = await _userService.GetUserByIdAsync(userId);
                var userFullName = user?.FullName ?? userName;

                // Tạo review mới
                var review = new ProductReview
                {
                    product_id = productId,
                    user_id = userId,
                    Rating = rating,
                    Comment = comment,
                    full_name = userFullName, // Lưu full_name từ User
                    CreatedAt = DateTime.Now
                };

                // Lưu vào database
                await _reviewService.AddReviewAsync(review);

                // Trả về JSON với review mới
                return Json(new
                {
                    success = true,
                    review = new
                    {
                        full_name = userFullName,
                        rating = review.Rating,
                        comment = review.Comment,
                        createdAt = review.CreatedAt.ToString("MMM dd, yyyy")
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Error adding review: " + ex.Message
                });
            }
        }


    }
}
