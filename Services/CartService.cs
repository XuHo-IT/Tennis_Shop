using BussinessObject;
using Repositories;

namespace Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task<Cart?> GetCartByIdAsync(int cartId)
        {
            return await _cartRepository.GetCartByIdAsync(cartId);
        }

        public async Task<Cart> GetOrCreateCartAsync(int userId)
        {
            // Try to get existing cart
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            
            // If cart doesn't exist, create new one
            if (cart == null)
            {
                cart = await _cartRepository.CreateCartAsync(userId);
            }

            return cart;
        }

        public async Task<CartItem> AddItemToCartAsync(int userId, int productId, int quantity, int? variantId = null)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID");

            if (productId <= 0)
                throw new ArgumentException("Invalid product ID");

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            // Get or create cart for user
            var cart = await GetOrCreateCartAsync(userId);

            // Add item to cart
            return await _cartRepository.AddItemToCartAsync(cart.Id, productId, quantity, variantId);
        }

        public async Task<CartItem?> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            if (cartItemId <= 0)
                throw new ArgumentException("Invalid cart item ID");

            return await _cartRepository.UpdateCartItemQuantityAsync(cartItemId, quantity);
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartItemId)
        {
            if (cartItemId <= 0)
                throw new ArgumentException("Invalid cart item ID");

            return await _cartRepository.RemoveItemFromCartAsync(cartItemId);
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                return false;

            return await _cartRepository.ClearCartAsync(cart.Id);
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            if (userId <= 0)
                return 0;

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                return 0;

            return await _cartRepository.GetCartItemCountAsync(cart.Id);
        }

        public async Task<decimal> GetCartTotalAsync(int userId)
        {
            if (userId <= 0)
                return 0;

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
                return 0;

            return await _cartRepository.GetCartTotalAsync(cart.Id);
        }
    }
}
