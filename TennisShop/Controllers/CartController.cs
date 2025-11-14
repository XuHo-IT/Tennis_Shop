using BussinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
                var userId = GetCurrentUserId();
                
                // Validate product exists
                var product = await _productService.GetProductByIdAsync(productId);
                if (product == null)
                {
                    TempData["Error"] = "Product not found";
                    return RedirectToAction("Index", "Product");
                }

                // If product has variants, validate variant selection
                if (product.ProductVariants != null && product.ProductVariants.Any())
                {
                    if (!variantId.HasValue || variantId.Value == 0)
                    {
                        TempData["Error"] = "Please select a product variant";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    // Validate variant exists and has stock
                    var variant = product.ProductVariants.FirstOrDefault(v => v.Id == variantId.Value);
                    if (variant == null)
                    {
                        TempData["Error"] = "Selected variant not found";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    if (variant.Stock < quantity)
                    {
                        TempData["Error"] = $"Insufficient stock for selected variant. Only {variant.Stock} available";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    TempData["Success"] = $"{product.Name} ({variant.Color} - {variant.Size}) added to cart successfully!";
                }
                else
                {
                    // Check stock for regular product without variants
                    if (product.Stock < quantity)
                    {
                        TempData["Error"] = "Insufficient stock available";
                        return RedirectToAction("Details", "Product", new { id = productId });
                    }

                    TempData["Success"] = $"{product.Name} added to cart successfully!";
                }

                await _cartService.AddItemToCartAsync(userId, productId, quantity, variantId);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error adding item to cart: " + ex.Message;
                return RedirectToAction("Index", "Product");
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
