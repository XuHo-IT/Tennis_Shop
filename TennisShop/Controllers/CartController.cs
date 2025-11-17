using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services;
using System.Security.Claims;

namespace TennisShop.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // Override OnActionExecuting to handle AJAX requests that get 401
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            // If authorization failed and this is an AJAX request, return JSON instead of redirect
            if (context.Result is Microsoft.AspNetCore.Mvc.ChallengeResult || 
                context.Result is Microsoft.AspNetCore.Mvc.ForbidResult)
            {
                bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                             Request.Headers["Accept"].ToString().Contains("application/json");
                
                if (isAjax)
                {
                    context.Result = Json(new { success = false, requiresAuth = true, message = "Please login to add items to cart" });
                }
            }
        }

        // Helper method to get current user ID
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User not authenticated");
        }

        // GET: Cart/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var cart = await _cartService.GetCartByUserIdAsync(userId);

                if (cart == null)
                {
                    // Create empty cart view
                    cart = new Cart { CartItems = new List<CartItem>() };
                }

                return View(cart);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading cart: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Cart/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(int productId, int quantity = 1, int? variantId = null)
        {
            try
            {
                // Check ModelState for validation errors
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    var errorMessage = string.Join(", ", errors);
                    
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                    {
                        return Json(new { success = false, message = $"Validation error: {errorMessage}" });
                    }
                    TempData["Error"] = $"Validation error: {errorMessage}";
                    return RedirectToAction("Details", "Product", new { id = productId });
                }
                
                var userId = GetCurrentUserId();
                
                // Validate product exists
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                    {
                        return Json(new { success = false, message = "Product not found" });
                    }
                    TempData["Error"] = "Product not found";
                    return RedirectToAction("Index", "Product");
                }

                // If product has variants, validate variant selection
                bool hasVariants = product.ProductVariants != null && product.ProductVariants.Any();
                
                if (hasVariants)
                {
                    if (!variantId.HasValue || variantId.Value == 0)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                        {
                            return Json(new { success = false, message = "Please select a product variant" });
                        }
                        TempData["Error"] = "Please select a product variant";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    // Validate variant exists and has stock
                    var variant = product.ProductVariants.FirstOrDefault(v => v.Id == variantId.Value);
                    if (variant == null)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                        {
                            return Json(new { success = false, message = "Selected variant not found" });
                        }
                        TempData["Error"] = "Selected variant not found";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    var variantStock = variant.Stock ?? 0;
                    if (variantStock < quantity)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                        {
                            return Json(new { success = false, message = $"Insufficient stock for selected variant. Only {variantStock} available" });
                        }
                        TempData["Error"] = $"Insufficient stock for selected variant. Only {variantStock} available";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    await _cartService.AddItemToCartAsync(userId, productId, quantity, variantId);
                    
                    // Check if this is an AJAX request
                    bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json");
                    if (isAjax)
                    {
                        var cartCount = await _cartService.GetCartItemCountAsync(userId);
                        return Json(new { 
                            success = true, 
                            message = $"{product.Name} ({variant.Color} - {variant.Size}) added to cart successfully!",
                            cartCount = cartCount
                        });
                    }
                    
                    TempData["Success"] = $"{product.Name} ({variant.Color} - {variant.Size}) added to cart successfully!";
                }
                else
                {
                    // For products without variants, set variantId to null
                    int? finalVariantId = null;
                    
                    // Check stock for regular product without variants
                    var productStock = product.Stock ?? 0;
                    if (productStock < quantity)
                    {
                        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                        {
                            return Json(new { success = false, message = $"Insufficient stock available. Only {productStock} available" });
                        }
                        TempData["Error"] = $"Insufficient stock available. Only {productStock} available";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    await _cartService.AddItemToCartAsync(userId, productId, quantity, finalVariantId);
                    
                    // Check if this is an AJAX request
                    bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json");
                    if (isAjax)
                    {
                        var cartCount = await _cartService.GetCartItemCountAsync(userId);
                        return Json(new { 
                            success = true, 
                            message = $"{product.Name} added to cart successfully!",
                            cartCount = cartCount
                        });
                    }
                    
                    TempData["Success"] = $"{product.Name} added to cart successfully!";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the full exception for debugging
                System.Diagnostics.Debug.WriteLine($"Error adding item to cart: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                var errorMessage = "Error adding item to cart: " + ex.Message;
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json"))
                {
                    return Json(new { success = false, message = errorMessage });
                }
                TempData["Error"] = errorMessage;
                return RedirectToAction("Details", "Product", new { id = productId });
            }
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                {
                    return Json(new { success = false, message = "Quantity must be greater than 0" });
                }

                var updatedItem = await _cartService.UpdateCartItemQuantityAsync(cartItemId, quantity);
                
                if (updatedItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                // Calculate line total
                var lineTotal = updatedItem.Quantity * updatedItem.UnitPrice;

                return Json(new 
                { 
                    success = true, 
                    quantity = updatedItem.Quantity,
                    lineTotal = lineTotal,
                    message = "Cart updated successfully!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error updating cart: " + ex.Message });
            }
        }

        // POST: Cart/RemoveItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                await _cartService.RemoveItemFromCartAsync(cartItemId);
                TempData["Success"] = "Item removed from cart";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error removing item: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Cart/Clear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            try
            {
                var userId = GetCurrentUserId();
                await _cartService.ClearCartAsync(userId);
                TempData["Success"] = "Cart cleared successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error clearing cart: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Cart/GetItemCount - For AJAX calls
        public async Task<IActionResult> GetItemCount()
        {
            try
            {
                var userId = GetCurrentUserId();
                var count = await _cartService.GetCartItemCountAsync(userId);
                return Json(new { count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }
    }
}
