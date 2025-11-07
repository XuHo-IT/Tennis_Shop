using Microsoft.EntityFrameworkCore;
using DataAccessLayer.Models;

namespace DataAccessLayer
{
    public class CartDAO
    {
        private readonly SportManagementContext _context;

        public CartDAO(SportManagementContext context)
        {
            _context = context;
        }

        // Get cart by user ID
        public async Task<Carts?> GetCartByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Variant)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        // Get cart by ID
        public async Task<Carts?> GetCartByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        // Create new cart for user
        public async Task<Carts> CreateCartAsync(int userId)
        {
            var cart = new Carts
            {
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        // Add item to cart
        public async Task<CartItems> AddItemToCartAsync(int cartId, int productId, int quantity, int? variantId = null)
        {
            // Check if item already exists in cart with same product and variant
            var existingItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId && ci.VariantId == variantId);

            if (existingItem != null)
            {
                // Update quantity if item exists
                existingItem.Quantity += quantity;
                await _context.SaveChangesAsync();
                return existingItem;
            }

            // Get product to set unit price
            var product = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == productId);
            
            if (product == null)
                throw new Exception("Product not found");

            // Determine unit price (from variant if specified, otherwise base price)
            decimal unitPrice = product.BasePrice;
            if (variantId.HasValue)
            {
                var variant = product.ProductVariants.FirstOrDefault(v => v.Id == variantId.Value);
                if (variant != null && variant.Price.HasValue)
                {
                    unitPrice = variant.Price.Value;
                }
            }

            // Create new cart item
            var cartItem = new CartItems
            {
                CartId = cartId,
                ProductId = productId,
                VariantId = variantId,
                Quantity = quantity,
                UnitPrice = unitPrice
            };

            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        // Update cart item quantity
        public async Task<CartItems?> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return null;

            if (quantity <= 0)
            {
                _context.CartItems.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
            }

            await _context.SaveChangesAsync();
            return cartItem;
        }

        // Remove item from cart
        public async Task<bool> RemoveItemFromCartAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
                return false;

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }

        // Clear all items in cart
        public async Task<bool> ClearCartAsync(int cartId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get cart item count
        public async Task<int> GetCartItemCountAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.Quantity);
        }

        // Get cart total amount
        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.Quantity * ci.UnitPrice);
        }
    }
}
