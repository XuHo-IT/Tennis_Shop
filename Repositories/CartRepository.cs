using DataAccessLayer;
using DataAccessLayer.Models;

namespace Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly CartDAO _cartDAO;

        public CartRepository(CartDAO cartDAO)
        {
            _cartDAO = cartDAO;
        }

        public async Task<Carts?> GetCartByUserIdAsync(int userId)
        {
            return await _cartDAO.GetCartByUserIdAsync(userId);
        }

        public async Task<Carts?> GetCartByIdAsync(int cartId)
        {
            return await _cartDAO.GetCartByIdAsync(cartId);
        }

        public async Task<Carts> CreateCartAsync(int userId)
        {
            return await _cartDAO.CreateCartAsync(userId);
        }

        public async Task<CartItems> AddItemToCartAsync(int cartId, int productId, int quantity, int? variantId = null)
        {
            return await _cartDAO.AddItemToCartAsync(cartId, productId, quantity, variantId);
        }

        public async Task<CartItems?> UpdateCartItemQuantityAsync(int cartItemId, int quantity)
        {
            return await _cartDAO.UpdateCartItemQuantityAsync(cartItemId, quantity);
        }

        public async Task<bool> RemoveItemFromCartAsync(int cartItemId)
        {
            return await _cartDAO.RemoveItemFromCartAsync(cartItemId);
        }

        public async Task<bool> ClearCartAsync(int cartId)
        {
            return await _cartDAO.ClearCartAsync(cartId);
        }

        public async Task<int> GetCartItemCountAsync(int cartId)
        {
            return await _cartDAO.GetCartItemCountAsync(cartId);
        }

        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            return await _cartDAO.GetCartTotalAsync(cartId);
        }
    }
}
