using DataAccessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;

namespace TennisShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IImageKitService _imageKitService;

        public ProductController(IProductService productService, IImageKitService imageKitService)
        {
            _productService = productService;
            _imageKitService = imageKitService;
        }

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
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
            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        [Authorize(Roles = "admin")]
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
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var allProducts = await _productService.GetAllProductsAsync();
                return View("Index", allProducts);
            }

            var products = await _productService.SearchProductsAsync(searchTerm);
            ViewBag.SearchTerm = searchTerm;
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

    }
}
